namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepthAndChangeColName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Order", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "Depth", c => c.Int(nullable: false));
            DropColumn("dbo.Comments", "CommentOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "CommentOrder", c => c.Int(nullable: false));
            DropColumn("dbo.Comments", "Depth");
            DropColumn("dbo.Comments", "Order");
        }
    }
}
