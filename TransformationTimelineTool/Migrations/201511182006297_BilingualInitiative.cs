namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BilingualInitiative : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Initiatives", "NameF", c => c.String());
            AddColumn("dbo.Initiatives", "DescriptionE", c => c.String());
            AddColumn("dbo.Initiatives", "DescriptionF", c => c.String());
            DropColumn("dbo.Initiatives", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Initiatives", "Description", c => c.String());
            DropColumn("dbo.Initiatives", "DescriptionF");
            DropColumn("dbo.Initiatives", "DescriptionE");
            DropColumn("dbo.Initiatives", "NameF");
        }
    }
}
