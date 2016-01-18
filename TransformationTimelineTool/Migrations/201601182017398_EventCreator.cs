namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventCreator : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "CreatorID", "dbo.AspNetUsers");
            DropIndex("dbo.Events", new[] { "CreatorID" });
            AlterColumn("dbo.Events", "CreatorID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Events", "CreatorID");
            AddForeignKey("dbo.Events", "CreatorID", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "CreatorID", "dbo.AspNetUsers");
            DropIndex("dbo.Events", new[] { "CreatorID" });
            AlterColumn("dbo.Events", "CreatorID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Events", "CreatorID");
            AddForeignKey("dbo.Events", "CreatorID", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
