namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitaitvesUsersManyToMany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InitiativeUsers",
                c => new
                    {
                        Initiative_ID = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Initiative_ID, t.User_Id })
                .ForeignKey("dbo.Initiatives", t => t.Initiative_ID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Initiative_ID)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InitiativeUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.InitiativeUsers", "Initiative_ID", "dbo.Initiatives");
            DropIndex("dbo.InitiativeUsers", new[] { "User_Id" });
            DropIndex("dbo.InitiativeUsers", new[] { "Initiative_ID" });
            DropTable("dbo.InitiativeUsers");
        }
    }
}
