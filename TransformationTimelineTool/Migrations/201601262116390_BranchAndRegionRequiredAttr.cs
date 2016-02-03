namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BranchAndRegionRequiredAttr : DbMigration
    {
        public override void Up()
        {
            var updateValue = "'[Temp value - needs updating]'";

            Sql("UPDATE dbo.Branches SET NameShort = " + updateValue + " WHERE NameShort IS NULL");
            Sql("UPDATE dbo.Branches SET NameE = " + updateValue + " WHERE NameE IS NULL");
            Sql("UPDATE dbo.Branches SET NameF = " + updateValue + " WHERE NameF IS NULL");

            Sql("UPDATE dbo.Regions SET NameShort = " + updateValue + " WHERE NameShort IS NULL");
            Sql("UPDATE dbo.Regions SET NameE = " + updateValue + " WHERE NameE IS NULL");
            Sql("UPDATE dbo.Regions SET NameF = " + updateValue + " WHERE NameF IS NULL");

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
