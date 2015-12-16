namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegionBranchRelation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegionBranches",
                c => new
                    {
                        Region_ID = c.Int(nullable: false),
                        Branch_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Region_ID, t.Branch_ID })
                .ForeignKey("dbo.Regions", t => t.Region_ID, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.Branch_ID, cascadeDelete: true)
                .Index(t => t.Region_ID)
                .Index(t => t.Branch_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RegionBranches", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.RegionBranches", "Region_ID", "dbo.Regions");
            DropIndex("dbo.RegionBranches", new[] { "Branch_ID" });
            DropIndex("dbo.RegionBranches", new[] { "Region_ID" });
            DropTable("dbo.RegionBranches");
        }
    }
}
