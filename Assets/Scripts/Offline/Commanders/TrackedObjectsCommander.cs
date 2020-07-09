﻿using Elektronik.Common.Commands;
using Elektronik.Common.Commands.Generic;
using Elektronik.Common.Data.Pb;
using Elektronik.Common.Maps;
using System.Collections.Generic;
using System.Linq;

namespace Elektronik.Offline.Commanders
{
    public class TrackedObjectsCommander : Commander
    {
        public SlamMap map;
        
        protected virtual ICommand GetCommand(IEnumerable<TrackedObjPb> objs, PacketPb.Types.ActionType action)
        {
            switch (action)
            {
                case PacketPb.Types.ActionType.Add:
                    return new AddCommand<TrackedObjPb>(map.TrackedObjs, objs);
                case PacketPb.Types.ActionType.Update:
                    return new UpdateTrackedObjsCommand(map.TrackedObjsGO, objs);
                case PacketPb.Types.ActionType.Remove:
                    return new RemoveTrackedObjCommands(map.TrackedObjsGO, objs);
                case PacketPb.Types.ActionType.Clear:
                    return new ClearTrackedObjsCommand(map.TrackedObjsGO);
                default: return null;
            }
        }

        public override void GetCommands(PacketPb pkg, in LinkedList<ICommand> commands)
        {
            if (pkg.DataCase == PacketPb.DataOneofCase.TrackedObjs)
            {
                var command = GetCommand(pkg.TrackedObjs.Data, pkg.Action);
                command.Execute();
                commands.AddLast(command);
            }
            base.GetCommands(pkg, commands);
        }
    }
}
