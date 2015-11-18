namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BilingualRegionBranch : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.Events", "Region_ID", "dbo.Regions");
            DropIndex("dbo.Events", new[] { "Branch_ID" });
            DropIndex("dbo.Events", new[] { "Region_ID" });
            RenameColumn(table: "dbo.Events", name: "Branch_ID", newName: "BranchID");
            RenameColumn(table: "dbo.Events", name: "Region_ID", newName: "RegionID");
            AddColumn("dbo.Branches", "NameE", c => c.String());
            AddColumn("dbo.Branches", "NameF", c => c.String());
            AddColumn("dbo.Regions", "NameE", c => c.String());
            AddColumn("dbo.Regions", "NameF", c => c.String());
            AlterColumn("dbo.Events", "BranchID", c => c.Int(nullable: false));
            AlterColumn("dbo.Events", "RegionID", c => c.Int(nullable: false));
            CreateIndex("dbo.Events", "BranchID");
            CreateIndex("dbo.Events", "RegionID");
            AddForeignKey("dbo.Events", "BranchID", "dbo.Branches", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Events", "RegionID", "dbo.Regions", "ID", cascadeDelete: true);
            DropColumn("dbo.Branches", "Name");
            DropColumn("dbo.Regions", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Regions", "Name", c => c.String());
            AddColumn("dbo.Branches", "Name", c => c.String());
            DropForeignKey("dbo.Events", "RegionID", "dbo.Regions");
            DropForeignKey("dbo.Events", "BranchID", "dbo.Branches");
            DropIndex("dbo.Events", new[] { "RegionID" });
            DropIndex("dbo.Events", new[] { "BranchID" });
            AlterColumn("dbo.Events", "RegionID", c => c.Int());
            AlterColumn("dbo.Events", "BranchID", c => c.Int());
            DropColumn("dbo.Regions", "NameF");
            DropColumn("dbo.Regions", "NameE");
            DropColumn("dbo.Branches", "NameF");
            DropColumn("dbo.Branches", "NameE");
            RenameColumn(table: "dbo.Events", name: "RegionID", newName: "Region_ID");
            RenameColumn(table: "dbo.Events", name: "BranchID", newName: "Branch_ID");
            CreateIndex("dbo.Events", "Region_ID");
            CreateIndex("dbo.Events", "Branch_ID");
            AddForeignKey("dbo.Events", "Region_ID", "dbo.Regions", "ID");
            AddForeignKey("dbo.Events", "Branch_ID", "dbo.Branches", "ID");
        }
    }
}
