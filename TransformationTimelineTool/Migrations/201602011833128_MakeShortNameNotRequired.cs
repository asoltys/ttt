namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeShortNameNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Branches", "NameShort", c => c.String());
            AlterColumn("dbo.Regions", "NameShort", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Regions", "NameShort", c => c.String(nullable: false));
            AlterColumn("dbo.Branches", "NameShort", c => c.String(nullable: false));
        }
    }
}
