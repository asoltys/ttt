namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserRegionsBranches : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ImpactRegions", newName: "RegionImpacts");
            DropForeignKey("dbo.UserBranches", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserBranches", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.RegionUsers", "Region_ID", "dbo.Regions");
            DropForeignKey("dbo.RegionUsers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserBranches", new[] { "User_Id" });
            DropIndex("dbo.UserBranches", new[] { "Branch_ID" });
            DropIndex("dbo.RegionUsers", new[] { "Region_ID" });
            DropIndex("dbo.RegionUsers", new[] { "User_Id" });
            DropPrimaryKey("dbo.RegionImpacts");
            AddPrimaryKey("dbo.RegionImpacts", new[] { "Region_ID", "Impact_ID" });
            DropTable("dbo.UserBranches");
            DropTable("dbo.RegionUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RegionUsers",
                c => new
                    {
                        Region_ID = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Region_ID, t.User_Id });
            
            CreateTable(
                "dbo.UserBranches",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Branch_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Branch_ID });
            
            DropPrimaryKey("dbo.RegionImpacts");
            AddPrimaryKey("dbo.RegionImpacts", new[] { "Impact_ID", "Region_ID" });
            CreateIndex("dbo.RegionUsers", "User_Id");
            CreateIndex("dbo.RegionUsers", "Region_ID");
            CreateIndex("dbo.UserBranches", "Branch_ID");
            CreateIndex("dbo.UserBranches", "User_Id");
            AddForeignKey("dbo.RegionUsers", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RegionUsers", "Region_ID", "dbo.Regions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.UserBranches", "Branch_ID", "dbo.Branches", "ID", cascadeDelete: true);
            AddForeignKey("dbo.UserBranches", "User_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.RegionImpacts", newName: "ImpactRegions");
        }
    }
}
