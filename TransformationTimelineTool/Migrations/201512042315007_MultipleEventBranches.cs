namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultipleEventBranches : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "BranchID", "dbo.Branches");
            DropIndex("dbo.Events", new[] { "BranchID" });
            CreateTable(
                "dbo.EventBranches",
                c => new
                    {
                        Event_ID = c.Int(nullable: false),
                        Branch_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_ID, t.Branch_ID })
                .ForeignKey("dbo.Events", t => t.Event_ID, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.Branch_ID, cascadeDelete: true)
                .Index(t => t.Event_ID)
                .Index(t => t.Branch_ID);
            
            DropColumn("dbo.Events", "BranchID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "BranchID", c => c.Int(nullable: false));
            DropForeignKey("dbo.EventBranches", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.EventBranches", "Event_ID", "dbo.Events");
            DropIndex("dbo.EventBranches", new[] { "Branch_ID" });
            DropIndex("dbo.EventBranches", new[] { "Event_ID" });
            DropTable("dbo.EventBranches");
            CreateIndex("dbo.Events", "BranchID");
            AddForeignKey("dbo.Events", "BranchID", "dbo.Branches", "ID", cascadeDelete: true);
        }
    }
}
