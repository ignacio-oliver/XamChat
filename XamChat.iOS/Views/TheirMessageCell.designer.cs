// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace XamChat.iOS
{
    [Register ("TheirMessageCell")]
    partial class TheirMessageCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel theirMessage { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (theirMessage != null) {
                theirMessage.Dispose ();
                theirMessage = null;
            }
        }
    }
}