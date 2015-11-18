namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Branches",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NameShort = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        InitiativeID = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Text = c.String(),
                        Hover = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Initiatives", t => t.InitiativeID, cascadeDelete: true)
                .Index(t => t.InitiativeID);
            
            CreateTable(
                "dbo.Initiatives",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Impacts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Branch_ID = c.Int(),
                        Initiative_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Branches", t => t.Branch_ID)
                .ForeignKey("dbo.Initiatives", t => t.Initiative_ID)
                .Index(t => t.Branch_ID)
                .Index(t => t.Initiative_ID);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NameShort = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Impacts", "Initiative_ID", "dbo.Initiatives");
            DropForeignKey("dbo.Impacts", "Branch_ID", "dbo.Branches");
            DropForeignKey("dbo.Events", "InitiativeID", "dbo.Initiatives");
            DropIndex("dbo.Impacts", new[] { "Initiative_ID" });
            DropIndex("dbo.Impacts", new[] { "Branch_ID" });
            DropIndex("dbo.Events", new[] { "InitiativeID" });
            DropTable("dbo.Regions");
            DropTable("dbo.Impacts");
            DropTable("dbo.Initiatives");
            DropTable("dbo.Events");
            DropTable("dbo.Branches");
        }
    }
}
