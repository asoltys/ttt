namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserBranchesRegions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Branches", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Regions", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Branches", new[] { "User_Id" });
            DropIndex("dbo.Regions", new[] { "User_Id" });
            CreateTable(
                "dbo.UserBranches",
                c => new
                    {
                        User_Id = c.String(nullable: false, maxLength: 128),
                        Branch_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Branch_ID })
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Branches", t => t.Branch_ID, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Branch_ID);
            
            CreateTable(
                "dbo.RegionUsers",
                c => new
                    {
                        Region_ID = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Region_ID, t.User_Id })
                .ForeignKey("dbo.Regions", t => t.Region_ID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Region_ID)
                .Index(t => t.User_Id);
            
            DropColumn("dbo.Branches", "User_Id");
            DropColumn("dbo.Regions", "User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Regions", "User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Branches", "User_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.RegionUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.RegionUsers", "Region_ID", "dbo.Regions");
            DropForeignKey("dbo.UserBranches", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.UserBranches", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.RegionUsers", new[] { "User_Id" });
            DropIndex("dbo.RegionUsers", new[] { "Region_ID" });
            DropIndex("dbo.UserBranches", new[] { "Branch_ID" });
            DropIndex("dbo.UserBranches", new[] { "User_Id" });
            DropTable("dbo.RegionUsers");
            DropTable("dbo.UserBranches");
            CreateIndex("dbo.Regions", "User_Id");
            CreateIndex("dbo.Branches", "User_Id");
            AddForeignKey("dbo.Regions", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Branches", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
