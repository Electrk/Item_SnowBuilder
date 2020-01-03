package Item_SnowBuilder
{
	function Armor::onTrigger ( %this, %player, %slot, %isDown )
	{
		if ( %player.getMountedImage (0) == SnowBuilderImage.getID ()  &&  %slot == 4  &&  %isDown )
		{
			%player.placeSnow = true;
			%player.setImageTrigger (0, true);

			%player.placeSnow = false;
			%player.setImageTrigger (0, false);
		}

		Parent::onTrigger (%this, %player, %slot, %isDown);
	}
};
activatePackage (Item_SnowBuilder);
