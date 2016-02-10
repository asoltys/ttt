namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimelineType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Initiatives", "Timeline", c => c.String(nullable: false, defaultValueSql: "'TransformationTimeline'"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Initiatives", "Timeline");
        }
    }
}
