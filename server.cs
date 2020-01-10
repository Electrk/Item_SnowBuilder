if ( LoadRequiredAddOn ("Support_BuildableSnow") == $Error::None  &&
	 BuildableSnow_RequireMinVersion ("0.9.0") )
{
	exec ("./Item_SnowBuilder.cs");
	exec ("./utility.cs");
	exec ("./package.cs");
}
else
{
	error ("ERROR: Missing required add-on `Support_BuildableSnow`!");
}
