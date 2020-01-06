if ( LoadRequiredAddOn ("Support_BuildableSnow") == $Error::None )
{
	exec ("./Item_SnowBuilder.cs");
	exec ("./utility.cs");
	exec ("./package.cs");
}
else
{
	error ("Item_SnowBuilder - Missing required add-on: Support_BuildableSnow");
	messageAll ('', "ERROR: Item_SnowBuilder - Missing required add-on: Support_BuildableSnow");
}
