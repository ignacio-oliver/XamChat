﻿using Foundation;
using System;
using UIKit;
using XamChat.Core;

namespace XamChat.iOS
{
    public partial class ConversationsController : UITableViewController
    {
        readonly MessageViewModel messageViewModel = ServiceContainer.Resolve<MessageViewModel>();

        public ConversationsController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new TableSource(this);
        }

        public async override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            try
            {
                await messageViewModel.GetConversations();
                TableView.ReloadData();
            }
            catch(Exception exc)
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
            const string CellName = "ConversationCell";
            readonly MessageViewModel messageViewModel = ServiceContainer.Resolve<MessageViewModel>();
            readonly ConversationsController controller;

            public TableSource(ConversationsController controller)
            {
                this.controller = controller;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return messageViewModel.Conversations == null ? 0 : messageViewModel.Conversations.Length;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var conversation = messageViewModel.Conversations[indexPath.Row];
                var cell = tableView.DequeueReusableCell(CellName);
                if (cell == null)
                {
                    //cell = new UITableViewCell(UITableViewCellStyle.Default, CellName);
                    cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellName);
                    cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                }
                cell.TextLabel.Text = conversation.Username;
                cell.DetailTextLabel.Text = conversation.LastMessage;
                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                var conversation = messageViewModel.Conversations[indexPath.Row];
                messageViewModel.Conversation = conversation;
                controller.PerformSegue("OnConversation", controller);
            }
        }
    }
}