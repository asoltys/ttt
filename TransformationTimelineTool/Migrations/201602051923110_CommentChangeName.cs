namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentChangeName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "CommentOrder", c => c.Int(nullable: false));
            DropColumn("dbo.Comments", "ReplyLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "ReplyLevel", c => c.Int(nullable: false));
            DropColumn("dbo.Comments", "CommentOrder");
        }
    }
}
