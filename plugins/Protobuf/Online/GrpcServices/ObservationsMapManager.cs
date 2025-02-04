﻿using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Elektronik.Containers;
using Elektronik.Data.Converters;
using Elektronik.Data.PackageObjects;
using Elektronik.Protobuf.Data;
using Grpc.Core;

namespace Elektronik.Protobuf.Online.GrpcServices
{
    public class ObservationsMapManager : ConnectableObjectsMapManager<SlamObservation, SlamObservationDiff>
    {
        private readonly ICSConverter _converter;

        public ObservationsMapManager(IConnectableObjectsContainer<SlamObservation> container, ICSConverter converter) :
                base(container)
        {
            _converter = converter;
        }

        public override Task<ErrorStatusPb> Handle(PacketPb request, ServerCallContext context)
        {
            if (request.DataCase != PacketPb.DataOneofCase.Observations) return base.Handle(request, context);
            Timer = Stopwatch.StartNew();
            var obs = request.ExtractObservations(_converter, Directory.GetCurrentDirectory()).ToList();
            return HandleConnections(request, Handle(request.Action, obs));
        }
    }
}