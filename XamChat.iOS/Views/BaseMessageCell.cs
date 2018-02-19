using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using XamChat.Core;

namespace XamChat.iOS
{
    public class BaseMessageCell : UITableViewCell
    {
        public BaseMessageCell(IntPtr handle) : base(handle)
        {
        }

        public virtual void Update(Message message)
        {

        }
    }
}