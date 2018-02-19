using Foundation;
using System;
using UIKit;
using XamChat.Core;

namespace XamChat.iOS
{
    public partial class FriendsController : UITableViewController
    {
        readonly FriendViewModel friendViewModel = ServiceContainer.Resolve<FriendViewModel>();

        public FriendsController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new TableSource();
        }

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                await friendViewModel.GetFriends();
                TableView.ReloadData();
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

        class TableSource : UITableViewSource
        {
            const string CellName = "FriendCell";
            readonly FriendViewModel friendViewModel = ServiceContainer.Resolve<FriendViewModel>();

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return friendViewModel.Friends == null ? 0 : friendViewModel.Friends.Length;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var friend = friendViewModel.Friends[indexPath.Row];
                var cell = tableView.DequeueReusableCell(CellName);
                if(cell == null)
                {
                    cell = new UITableViewCell(UITableViewCellStyle.Default, CellName);
                    cell.AccessoryView = UIButton.FromType(UIButtonType.ContactAdd);
                    cell.AccessoryView.UserInteractionEnabled = false;
                }
                cell.TextLabel.Text = friend.Username;
                return cell;
            }
        }
    }


}