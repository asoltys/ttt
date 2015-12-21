namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImpactsJustification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Impacts", "Justification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Impacts", "Justification");
        }
    }
}
