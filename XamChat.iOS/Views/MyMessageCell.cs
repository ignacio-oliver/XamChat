using Foundation;
using System;
using UIKit;
using XamChat.Core;

namespace XamChat.iOS
{
    public partial class MyMessageCell : BaseMessageCell
    {
        public MyMessageCell (IntPtr handle) : base (handle)
        {
        }

        public override void Update(Message message)
        {
            this.myMessage.Text = message.Text;
        }
    }
}