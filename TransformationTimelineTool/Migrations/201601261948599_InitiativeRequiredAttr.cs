namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitiativeRequiredAttr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Initiatives", "NameE", c => c.String(nullable: false));
            AlterColumn("dbo.Initiatives", "NameF", c => c.String(nullable: false));
            AlterColumn("dbo.Initiatives", "DescriptionE", c => c.String(nullable: false));
            AlterColumn("dbo.Initiatives", "DescriptionF", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Initiatives", "DescriptionF", c => c.String());
            AlterColumn("dbo.Initiatives", "DescriptionE", c => c.String());
            AlterColumn("dbo.Initiatives", "NameF", c => c.String());
            AlterColumn("dbo.Initiatives", "NameE", c => c.String());
        }
    }
}
