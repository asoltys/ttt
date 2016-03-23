namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitiativeEdited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Initiatives", "Edited", c => c.Boolean(nullable: true, defaultValue: false));
            Sql("update [dbo].[Initiatives] SET Edited = 'false' WHERE Edited IS NULL");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Initiatives", "Edited");
        }
    }
}
