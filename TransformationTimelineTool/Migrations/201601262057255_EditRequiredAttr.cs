namespace TransformationTimelineTool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditRequiredAttr : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Edits", "HoverE", c => c.String(nullable: false));
            AlterColumn("dbo.Edits", "HoverF", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Edits", "HoverF", c => c.String());
            AlterColumn("dbo.Edits", "HoverE", c => c.String());
        }
    }
}
