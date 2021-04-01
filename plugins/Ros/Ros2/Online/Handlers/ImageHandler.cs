﻿using Elektronik.RosPlugin.Common.Containers;

namespace Elektronik.RosPlugin.Ros2.Online.Handlers
{
    public class ImageHandler : MessageHandler
    {
        private readonly ImagePresenter _presenter;

        public ImageHandler(ImagePresenter presenter)
        {
            _presenter = presenter;
        }

        public override void Handle(Ros2Message message)
        {
            var image = message.CastTo<ImageMessage>();
            if (image is null) return;
            _presenter.Present(new ImagePresenter.ImageData((int) image.width, (int) image.height, image.encoding,
                                                            image.data.ToArray()));
            message.Dispose();
        }
    }
}