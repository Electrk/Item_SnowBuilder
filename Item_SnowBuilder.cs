$SnowBuilder::Version          = 1;
$SnowBuilder::DistanceCheck    = 1.75;
$SnowBuilder::DistanceCheckMin = $SnowBuilder::DistanceCheck;
$SnowBuilder::DistanceCheckMax = 999999;
$SnowBuilder::CenterHigherZ    = true;
$SnowBuilder::RaycastRange     = 6;

// Ugly hack because there's no type for fxPlanes that I'm aware of.
if ( $TypeMasks::FxPlaneObjectType $= "" )
{
	%temp = new fxPlane ();
	$TypeMasks::FxPlaneObjectType = %temp.getType ();
	%temp.delete ();
}

// ------------------------------------------------


datablock ItemData (SnowBuilderItem : HammerItem)
{
	image     = SnowBuilderImage;
	iconName  = expandFileName ("./icon");
	shapeFile = "base/data/shapes/empty.dts";

	doColorShift    = true;
	colorShiftColor = "0.3 0.3 0.3 1.0";

	uiName = "Snow Builder";
};

datablock ShapeBaseImageData (SnowBuilderImage)
{
	item      = SnowBuilderItem;
	shapeFile = "base/data/shapes/empty.dts";

	armReady = false;

	stateName[0]                    = "Activate";
	stateTimeoutValue[0]            = 0.15;
	stateTransitionOnTimeout[0]     = "Ready";

	stateName[1]                    = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1]        = true;
	stateSequence[1]                = "Ready";

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Wait";
	stateTimeoutValue[2]            = 0.14;
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]          = true;

	stateName[3]                    = "Wait";
	stateTimeoutValue[3]            = 0.01;
	stateTransitionOnTriggerUp[3]   = "Ready";
};

function SnowBuilderImage::onFire ( %this, %obj, %slot )
{
	if ( %obj.getClassName () !$= "Player"  ||  %obj.getDamagePercent () >= 1.0 )
	{
		return;
	}

	if ( %obj.getDamagePercent () < 1.0 )
	{
		%obj.playThread (2, activate);
	}

	%start = %obj.getEyePoint ();
	%eye   = %obj.getEyeVector ();
	%scale = getWord (%obj.getScale (), 2);
	%end   = vectorAdd (%start, VectorScale (%eye, $SnowBuilder::RaycastRange * %scale));

	// We don't look for non-raycasted bricks so it doesn't try to add/remove snow in mid-air.
	//
	// The downside to this is that we need to do an additional check with the fxPlane when we've
	// hit the ground and can't find any more snow bricks.
	%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::FxPlaneObjectType;

	%search = containerRayCast (%start, %end, %mask);
	%target = getWord (%search, 0);

	if ( %target )
	{
		%position = getWords (%search, 1, 3);

		// Since we're not searching for non-raycasted bricks, we need to do an additional search
		// from the fxPlane, so we can still place snow when we've reached the ground.
		if ( %target.getClassName () $= "fxPlane"  &&  %target.getName () $= "groundPlane" )
		{
			initContainerBoxSearch (%position, 0.01, $TypeMasks::FxBrickAlwaysObjectType);

			if ( !isObject (%brick = containerSearchNext ())  ||  !%brick.dataBlock.isSnowBrick )
			{
				return;
			}

			%target = %brick;
		}

		if ( %obj.placeSnow )
		{
			//* Prevent players from being able to place snow inside other players. *//

			initContainerRadiusSearch (%position, 1, $TypeMasks::PlayerObjectType);

			while ( isObject (%player = containerSearchNext ()) )
			{
				%playerScale = %player.getScale ();
				%center      = %player.getPlayerCenter ();
				%centerZ     = getWord (%center, 2);
				%positionZ   = getWord (%position, 2);

				%checkPos = %position;

				if ( $SnowBuilder::CenterHigherZ  &&  %positionZ > %centerZ )
				{
					%checkPos = getWords (%position, 0, 1) SPC %centerZ;
				}

				%scaleX = getWord (%playerScale, 0);
				%scaleY = getWord (%playerScale, 1);
				%scaleZ = getWord (%playerScale, 2);

				%distScale = (%scaleX + %scaleY + %scaleZ) / 3;
				%distCheck = mClampF (%distScale * $SnowBuilder::DistanceCheck,
					$SnowBuilder::DistanceCheckMin,
					$SnowBuilder::DistanceCheckMax);

				if ( vectorDist (%checkPos, %center) < %distCheck )
				{
					return;
				}
			}

			%target.raiseSnow ();
		}
		else
		{
			%target.lowerSnow ();
		}
	}
}
