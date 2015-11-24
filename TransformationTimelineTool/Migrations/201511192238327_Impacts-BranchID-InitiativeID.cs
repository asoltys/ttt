namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImpactsBranchIDInitiativeID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Impacts", "Initiative_ID", "dbo.Initiatives");
            DropForeignKey("dbo.Impacts", "Branch_ID", "dbo.Branches");
            DropIndex("dbo.Impacts", new[] { "Branch_ID" });
            DropIndex("dbo.Impacts", new[] { "Initiative_ID" });
            RenameColumn(table: "dbo.Impacts", name: "Initiative_ID", newName: "InitiativeID");
            RenameColumn(table: "dbo.Impacts", name: "Branch_ID", newName: "BranchID");
            AlterColumn("dbo.Impacts", "BranchID", c => c.Int(nullable: false));
            AlterColumn("dbo.Impacts", "InitiativeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Impacts", "InitiativeID");
            CreateIndex("dbo.Impacts", "BranchID");
            AddForeignKey("dbo.Impacts", "InitiativeID", "dbo.Initiatives", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Impacts", "BranchID", "dbo.Branches", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Impacts", "BranchID", "dbo.Branches");
            DropForeignKey("dbo.Impacts", "InitiativeID", "dbo.Initiatives");
            DropIndex("dbo.Impacts", new[] { "BranchID" });
            DropIndex("dbo.Impacts", new[] { "InitiativeID" });
            AlterColumn("dbo.Impacts", "InitiativeID", c => c.Int());
            AlterColumn("dbo.Impacts", "BranchID", c => c.Int());
            RenameColumn(table: "dbo.Impacts", name: "BranchID", newName: "Branch_ID");
            RenameColumn(table: "dbo.Impacts", name: "InitiativeID", newName: "Initiative_ID");
            CreateIndex("dbo.Impacts", "Initiative_ID");
            CreateIndex("dbo.Impacts", "Branch_ID");
            AddForeignKey("dbo.Impacts", "Branch_ID", "dbo.Branches", "ID");
            AddForeignKey("dbo.Impacts", "Initiative_ID", "dbo.Initiatives", "ID");
        }
    }
}
