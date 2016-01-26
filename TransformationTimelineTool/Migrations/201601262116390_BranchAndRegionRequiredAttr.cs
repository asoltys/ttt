namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BranchAndRegionRequiredAttr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Branches", "NameShort", c => c.String(nullable: false));
            AlterColumn("dbo.Branches", "NameE", c => c.String(nullable: false));
            AlterColumn("dbo.Branches", "NameF", c => c.String(nullable: false));
            AlterColumn("dbo.Regions", "NameShort", c => c.String(nullable: false));
            AlterColumn("dbo.Regions", "NameE", c => c.String(nullable: false));
            AlterColumn("dbo.Regions", "NameF", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Regions", "NameF", c => c.String());
            AlterColumn("dbo.Regions", "NameE", c => c.String());
            AlterColumn("dbo.Regions", "NameShort", c => c.String());
            AlterColumn("dbo.Branches", "NameF", c => c.String());
            AlterColumn("dbo.Branches", "NameE", c => c.String());
            AlterColumn("dbo.Branches", "NameShort", c => c.String());
        }
    }
}
