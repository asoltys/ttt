namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditedFromBooleanToDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Initiatives", "Edited", c => c.DateTime(nullable: false, defaultValue: DateTime.Now));
            AlterColumn("dbo.Impacts", "Edited", c => c.DateTime(nullable: false, defaultValue: DateTime.Now));
            AlterColumn("dbo.Edits", "Edited", c => c.DateTime(nullable: false, defaultValue: DateTime.Now));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Edits", "Edited", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Impacts", "Edited", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Initiatives", "Edited", c => c.Boolean(nullable: false));
        }
    }
}
