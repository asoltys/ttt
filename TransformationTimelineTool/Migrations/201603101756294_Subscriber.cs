namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Subscriber : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscribers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SubscriberInitiatives",
                c => new
                    {
                        Subscriber_ID = c.Int(nullable: false),
                        Initiative_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Subscriber_ID, t.Initiative_ID })
                .ForeignKey("dbo.Subscribers", t => t.Subscriber_ID, cascadeDelete: true)
                .ForeignKey("dbo.Initiatives", t => t.Initiative_ID, cascadeDelete: true)
                .Index(t => t.Subscriber_ID)
                .Index(t => t.Initiative_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriberInitiatives", "Initiative_ID", "dbo.Initiatives");
            DropForeignKey("dbo.SubscriberInitiatives", "Subscriber_ID", "dbo.Subscribers");
            DropIndex("dbo.SubscriberInitiatives", new[] { "Initiative_ID" });
            DropIndex("dbo.SubscriberInitiatives", new[] { "Subscriber_ID" });
            DropTable("dbo.SubscriberInitiatives");
            DropTable("dbo.Subscribers");
        }
    }
}
