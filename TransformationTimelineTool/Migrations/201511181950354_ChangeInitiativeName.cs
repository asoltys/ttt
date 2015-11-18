namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeInitiativeName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Initiatives", "NameE", c => c.String());
            DropColumn("dbo.Initiatives", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Initiatives", "Name", c => c.String());
            DropColumn("dbo.Initiatives", "NameE");
        }
    }
}
