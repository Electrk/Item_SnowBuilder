if ( LoadRequiredAddOn ("Support_BuildableSnow") == $Error::None )
{
	exec ("./Item_SnowBuilder.cs");
	exec ("./utility.cs");
	exec ("./package.cs");
}
else
{
	error ("ERROR: Missing required add-on `Support_BuildableSnow`!");
}
