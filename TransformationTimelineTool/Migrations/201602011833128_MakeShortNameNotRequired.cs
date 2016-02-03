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
            var updateValue = "'[Temp value - needs updating]'";

            Sql("UPDATE dbo.Branches SET NameShort = " + updateValue + " WHERE NameShort IS NULL");
            Sql("UPDATE dbo.Regions SET NameShort = " + updateValue + " WHERE NameShort IS NULL");

            AlterColumn("dbo.Regions", "NameShort", c => c.String(nullable: false));
            AlterColumn("dbo.Branches", "NameShort", c => c.String(nullable: false));
        }
    }
}
