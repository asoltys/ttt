namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentAddReplyLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "ReplyLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "ReplyLevel");
        }
    }
}
