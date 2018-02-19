using Foundation;
using System;
using UIKit;
using XamChat.Core;

namespace XamChat.iOS
{
    public partial class TheirMessageCell : BaseMessageCell
    {
        public TheirMessageCell (IntPtr handle) : base (handle)
        {
        }

        public override void Update(Message message)
        {
            this.theirMessage.Text = message.Text;
        }
    }
}