namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "Type");
        }
    }
}
