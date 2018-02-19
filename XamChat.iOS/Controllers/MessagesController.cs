using Foundation;
using System;
using System.Diagnostics;
using System.Drawing;
using UIKit;
using XamChat.Core;

namespace XamChat.iOS
{
    public partial class MessagesController : UITableViewController
    {
        readonly MessageViewModel messageViewModel = ServiceContainer.Resolve<MessageViewModel>();
        UIToolbar toolbar;
        UITextField message;
        UIBarButtonItem send;
        NSObject willShowObserver, willHideObserver;
        nfloat keyboardHeigth = 0;

        public MessagesController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Bar button item
            send = new UIBarButtonItem("Send", UIBarButtonItemStyle.Bordered, (sender, e) => Send());
            //send = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) => Send());

            //Text field
            message = new UITextField(new RectangleF(0, 0, (float)(TableView.Frame.Width - 84), 32))
            {
                BorderStyle = UITextBorderStyle.RoundedRect,
                ReturnKeyType = UIReturnKeyType.Send,
                ShouldReturn = _ =>
                {
                    Send();
                    return false;
                }
            };

            //Toolbar
            toolbar = new UIToolbar(new RectangleF(0, (float)(TableView.Frame.Height - 44), (float)TableView.Frame.Width, 44));
            toolbar.Items = new UIBarButtonItem[]
            {
                new UIBarButtonItem(message), send
            };

            NavigationController.View.AddSubview(toolbar);

            TableView.Source = new TableSource();
            TableView.TableFooterView = new UIView(new RectangleF(0, 0, (float)TableView.Frame.Width, 44))
            {
                BackgroundColor = UIColor.Clear
            };
        }

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Title = messageViewModel.Conversation.Username;

            //Keyboard notifications
            willShowObserver = UIKeyboard.Notifications.ObserveWillShow((sender, e) => OnKeyboardNotification(e));
            willHideObserver = UIKeyboard.Notifications.ObserveWillHide((sender, e) => OnKeyboardNotification(e));

            //IsBusy
            messageViewModel.IsBusyChanged += OnIsBusyChanged;
            
            try
            {
                await messageViewModel.GetMessages();
                TableView.ReloadData();
                message.BecomeFirstResponder();
            }
            catch (Exception exc)
            {
                var alert = new UIAlertView()
                {
                    Title = "Oops!",
                    Message = exc.Message,
                };
                alert.AddButton("Ok");
                alert.Show();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            //Unsubscribe notifications
            if(willShowObserver != null)
            {
                willShowObserver.Dispose();
                willShowObserver = null;
            }
            if(willHideObserver != null)
            {
                willHideObserver.Dispose();
                willHideObserver = null;
            }

            //IsBusy
            messageViewModel.IsBusyChanged -= OnIsBusyChanged;

            toolbar.RemoveFromSuperview();
            toolbar.Dispose();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            messageViewModel.Clear();
        }

        void OnIsBusyChanged(object sender, EventArgs e)
        {
            message.Enabled = send.Enabled = !messageViewModel.IsBusy;
        }

        async void Send()
        {
            //Just hide the keyboard if they didn't type anything
            if (string.IsNullOrEmpty(message.Text))
            {
                message.ResignFirstResponder();
                return;
            }

            //Set the text, send the message
            messageViewModel.Text = message.Text;
            await messageViewModel.SendMessage();

            //Clear the text field & view model
            message.Text = messageViewModel.Text = string.Empty;

            //Reload the table
            TableView.ReloadData();

            //Hide the keyboard
            message.ResignFirstResponder();

            //Scroll to end, to see the new message
            ScrollToEnd();
        }

        void OnKeyboardNotification(UIKeyboardEventArgs e)
        {
            //Check if the keyboard is becoming visible
            bool willShow = e.Notification.Name == UIKeyboard.WillShowNotification;

            //Start an animation, using values from the keyboard
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationDuration(e.AnimationDuration);
            UIView.SetAnimationCurve(e.AnimationCurve);

            //Calculate keyboard height, etc.
            if (willShow)
            {
                var keyboardFrame = e.FrameEnd;

                var frame = toolbar.Frame;
                frame.Y -= keyboardFrame.Height;
                toolbar.Frame = frame;

                keyboardHeigth = keyboardFrame.Height;
            }
            else
            {
                var keyboardFrame = e.FrameBegin;

                var frame = toolbar.Frame;
                frame.Y += keyboardFrame.Height;
                toolbar.Frame = frame;

                keyboardHeigth = 0;
            }

            //Commit the animation
            UIView.CommitAnimations();
            ScrollToEnd();
        }

        void ScrollToEnd()
        {
            var offset = TableView.ContentSize.Height - TableView.Frame.Height + keyboardHeigth;
            var offset_variant = toolbar.Frame.Height + 20;

            if ((offset + offset_variant) > 0)
            {
                TableView.ContentOffset = new PointF(0, (float)(offset));
            }
        }

        class TableSource : UITableViewSource
        {
            const string MyCellName = "MyCell";
            const string TheirCellName = "TheirCell";
            readonly MessageViewModel messageViewModel = ServiceContainer.Resolve<MessageViewModel>();
            readonly ISettings settings = ServiceContainer.Resolve<ISettings>();

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return messageViewModel.Messages == null ? 0 : messageViewModel.Messages.Length;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var message = messageViewModel.Messages[indexPath.Row];
                bool isMyMessage = message.UserId == settings.User.Id;
                var cell = tableView.DequeueReusableCell(isMyMessage ? MyCellName : TheirCellName) as BaseMessageCell;
                cell.Update(message);
                return cell;
            }
        }
    }
}