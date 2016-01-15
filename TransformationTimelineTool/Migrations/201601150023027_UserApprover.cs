namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserApprover : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ApproverID", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "ApproverID");
            AddForeignKey("dbo.AspNetUsers", "ApproverID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "ApproverID", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "ApproverID" });
            DropColumn("dbo.AspNetUsers", "ApproverID");
        }
    }
}
