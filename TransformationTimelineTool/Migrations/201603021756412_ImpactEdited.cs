namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImpactEdited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Impacts", "Edited", c => c.Boolean(nullable: true, defaultValue: false));
            Sql("update [dbo].[Impacts] SET Edited = 'false' WHERE Edited IS NULL");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Impacts", "Edited");
        }
    }
}
