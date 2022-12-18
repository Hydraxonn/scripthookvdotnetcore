//
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
//

namespace GTA
{
    public enum Bone
    {
        SkelRoot = 0x0,
        SkelPelvis = 0x2E28,
        SkelLeftThigh = 0xE39F,
        SkelLeftCalf = 0xF9BB,
        SkelLeftFoot = 0x3779,
        SkelLeftToe0 = 0x83C,
        EOLeftFoot = 0x84C5,
        EOLeftToe = 0x68BD,
        IKLeftFoot = 0xFEDD,
        PHLeftFoot = 0xE175,
        MHLeftKnee = 0xB3FE,
        SkelRightThigh = 0xCA72,
        SkelRightCalf = 0x9000,
        SkelRightFoot = 0xCC4D,
        SkelRightToe0 = 0x512D,
        EORightFoot = 0x1096,
        EORightToe = 0x7163,
        IKRightFoot = 0x8AAE,
        PHRightFoot = 0x60E6,
        MHRightKnee = 0x3FCF,
        RBLeftThighRoll = 0x5C57,
        RBRightThighRoll = 0x192A,
        SkelSpineRoot = 0xE0FD,
        SkelSpine0 = 0x5C01,
        SkelSpine1 = 0x60F0,
        SkelSpine2 = 0x60F1,
        SkelSpine3 = 0x60F2,
        SkelLeftClavicle = 0xFCD9,
        SkelLeftUpperArm = 0xB1C5,
        SkelLeftForearm = 0xEEEB,
        SkelLeftHand = 0x49D9,
        SkelLeftFinger00 = 0x67F2,
        SkelLeftFinger01 = 0xFF9,
        SkelLeftFinger02 = 0xFFA,
        SkelLeftFinger10 = 0x67F3,
        SkelLeftFinger11 = 0x1049,
        SkelLeftFinger12 = 0x104A,
        SkelLeftFinger20 = 0x67F4,
        SkelLeftFinger21 = 0x1059,
        SkelLeftFinger22 = 0x105A,
        SkelLeftFinger30 = 0x67F5,
        SkelLeftFinger31 = 0x1029,
        SkelLeftFinger32 = 0x102A,
        SkelLeftFinger40 = 0x67F6,
        SkelLeftFinger41 = 0x1039,
        SkelLeftFinger42 = 0x103A,
        PHLeftHand = 0xEB95,
        IKLeftHand = 0x8CBD,
        RBLeftForeArmRoll = 0xEE4F,
        RBLeftArmRoll = 0x1470,
        MHLeftElbow = 0x58B7,
        SkelRightClavicle = 0x29D2,
        SkelRightUpperArm = 0x9D4D,
        SkelRightForearm = 0x6E5C,
        SkelRightHand = 0xDEAD,
        SkelRightFinger00 = 0xE5F2,
        SkelRightFinger01 = 0xFA10,
        SkelRightFinger02 = 0xFA11,
        SkelRightFinger10 = 0xE5F3,
        SkelRightFinger11 = 0xFA60,
        SkelRightFinger12 = 0xFA61,
        SkelRightFinger20 = 0xE5F4,
        SkelRightFinger21 = 0xFA70,
        SkelRightFinger22 = 0xFA71,
        SkelRightFinger30 = 0xE5F5,
        SkelRightFinger31 = 0xFA40,
        SkelRightFinger32 = 0xFA41,
        SkelRightFinger40 = 0xE5F6,
        SkelRightFinger41 = 0xFA50,
        SkelRightFinger42 = 0xFA51,
        PHRightHand = 0x6F06,
        IKRightHand = 0x188E,
        RBRightForeArmRoll = 0xAB22,
        RBRightArmRoll = 0x90FF,
        MHRightElbow = 0xBB0,
        SkelNeck1 = 0x9995,
        SkelHead = 0x796E,
        IKHead = 0x322C,
        FacialRoot = 0xFE2C,
        FBLeftBrowOut000 = 0xE3DB,
        FBLeftLidUpper000 = 0xB2B6,
        FBLeftEye000 = 0x62AC,
        FBLeftCheekBone000 = 0x542E,
        FBLeftLipCorner000 = 0x74AC,
        FBRightLidUpper000 = 0xAA10,
        FBRightEye000 = 0x6B52,
        FBRightCheekBone000 = 0x4B88,
        FBRightBrowOut000 = 0x54C,
        FBRightLipCorner000 = 0x2BA6,
        FBBrowCentre000 = 0x9149,
        FBUpperLipRoot000 = 0x4ED2,
        FBUpperLip000 = 0xF18F,
        FBLeftLipTop000 = 0x4F37,
        FBRightLipTop000 = 0x4537,
        FBJaw000 = 0xB4A0,
        FBLowerLipRoot000 = 0x4324,
        FBLowerLip000 = 0x508F,
        FBLeftLipBot000 = 0xB93B,
        FBRightLipBot000 = 0xC33B,
        FBTongue000 = 0xB987,
        RBNeck1 = 0x8B93,
        SPRLeftBreast = 0xFC8E,
        SPRRightBreast = 0x885F,
        IKRoot = 0xDD1C,
        SkelNeck2 = 0x5FD4,
        SkelPelvis1 = 0xD003,
        SkelPelvisRoot = 0x45FC,
        SkelSADDLE = 0x9524,
        MHLeftCalfBack = 0x1013,
        MHLeftThighBack = 0x600D,
        SMLeftSkirt = 0xC419,
        MHRightCalfBack = 0xB013,
        MHRightThighBack = 0x51A3,
        SMRightSkirt = 0x7712,
        SMMBackSkirtRoll = 0xDBB,
        SMLeftBackSkirtRoll = 0x40B2,
        SMRightBackSkirtRoll = 0xC141,
        SMMFrontSkirtRoll = 0xCDBB,
        SMLeftFrontSkirtRoll = 0x9B69,
        SMRightFrontSkirtRoll = 0x86F1,
        SMCockNBallsRoot = 0xC67D,
        SMCockNBalls = 0x9D34,
        MHLeftFinger00 = 0x8C63,
        MHLeftFingerBulge00 = 0x5FB8,
        MHLeftFinger10 = 0x8C53,
        MHLeftFingerTop00 = 0xA244,
        MHLeftHandSide = 0xC78A,
        MHWatch = 0x2738,
        MHLeftSleeve = 0x933C,
        MHRightFinger00 = 0x2C63,
        MHRightFingerBulge00 = 0x69B8,
        MHRightFinger10 = 0x2C53,
        MHRightFingerTop00 = 0xEF4B,
        MHRightHandSide = 0x68FB,
        MHRightSleeve = 0x92DC,
        FacialJaw = 0xB21,
        FacialUnderChin = 0x8A95,
        FacialLeftUnderChin = 0x234E,
        FacialChin = 0xB578,
        FacialChinSkinBottom = 0x98BC,
        FacialLeftChinSkinBottom = 0x3E8F,
        FacialRightChinSkinBottom = 0x9E8F,
        FacialTongueA = 0x4A7C,
        FacialTongueB = 0x4A7D,
        FacialTongueC = 0x4A7E,
        FacialTongueD = 0x4A7F,
        FacialTongueE = 0x4A80,
        FacialLeftTongueE = 0x35F2,
        FacialRightTongueE = 0x2FF2,
        FacialLeftTongueD = 0x35F1,
        FacialRightTongueD = 0x2FF1,
        FacialLeftTongueC = 0x35F0,
        FacialRightTongueC = 0x2FF0,
        FacialLeftTongueB = 0x35EF,
        FacialRightTongueB = 0x2FEF,
        FacialLeftTongueA = 0x35EE,
        FacialRightTongueA = 0x2FEE,
        FacialChinSkinTop = 0x7226,
        FacialLeftChinSkinTop = 0x3EB3,
        FacialChinSkinMid = 0x899A,
        FacialLeftChinSkinMid = 0x4427,
        FacialLeftChinSide = 0x4A5E,
        FacialRightChinSkinMid = 0xF5AF,
        FacialRightChinSkinTop = 0xF03B,
        FacialRightChinSide = 0xAA5E,
        FacialRightUnderChin = 0x2BF4,
        FacialLeftLipLowerSDK = 0xB9E1,
        FacialLeftLipLowerAnalog = 0x244A,
        FacialLeftLipLowerThicknessV = 0xC749,
        FacialLeftLipLowerThicknessH = 0xC67B,
        FacialLipLowerSDK = 0x7285,
        FacialLipLowerAnalog = 0xD97B,
        FacialLipLowerThicknessV = 0xC5BB,
        FacialLipLowerThicknessH = 0xC5ED,
        FacialRightLipLowerSDK = 0xA034,
        FacialRightLipLowerAnalog = 0xC2D9,
        FacialRightLipLowerThicknessV = 0xC6E9,
        FacialRightLipLowerThicknessH = 0xC6DB,
        FacialNose = 0x20F1,
        FacialLeftNostril = 0x7322,
        FacialLeftNostrilThickness = 0xC15F,
        FacialNoseLower = 0xE05A,
        FacialLeftNoseLowerThickness = 0x79D5,
        FacialRightNoseLowerThickness = 0x7975,
        FacialNoseTip = 0x6A60,
        FacialRightNostril = 0x7922,
        FacialRightNostrilThickness = 0x36FF,
        FacialNoseUpper = 0xA04F,
        FacialLeftNoseUpper = 0x1FB8,
        FacialNoseBridge = 0x9BA3,
        FacialLeftNasolabialFurrow = 0x5ACA,
        FacialLeftNasolabialBulge = 0xCD78,
        FacialLeftCheekLower = 0x6907,
        FacialLeftCheekLowerBulge1 = 0xE3FB,
        FacialLeftCheekLowerBulge2 = 0xE3FC,
        FacialLeftCheekInner = 0xE7AB,
        FacialLeftCheekOuter = 0x8161,
        FacialLeftEyesackLower = 0x771B,
        FacialLeftEyeball = 0x1744,
        FacialLeftEyelidLower = 0x998C,
        FacialLeftEyelidLowerOuterSDK = 0xFE4C,
        FacialLeftEyelidLowerOuterAnalog = 0xB9AA,
        FacialLeftEyelashLowerOuter = 0xD7F6,
        FacialLeftEyelidLowerInnerSDK = 0xF151,
        FacialLeftEyelidLowerInnerAnalog = 0x8242,
        FacialLeftEyelashLowerInner = 0x4CCF,
        FacialLeftEyelidUpper = 0x97C1,
        FacialLeftEyelidUpperOuterSDK = 0xAF15,
        FacialLeftEyelidUpperOuterAnalog = 0x67FA,
        FacialLeftEyelashUpperOuter = 0x27B7,
        FacialLeftEyelidUpperInnerSDK = 0xD341,
        FacialLeftEyelidUpperInnerAnalog = 0xF092,
        FacialLeftEyelashUpperInner = 0x9B1F,
        FacialLeftEyesackUpperOuterBulge = 0xA559,
        FacialLeftEyesackUpperInnerBulge = 0x2F2A,
        FacialLeftEyesackUpperOuterFurrow = 0xC597,
        FacialLeftEyesackUpperInnerFurrow = 0x52A7,
        FacialForehead = 0x9218,
        FacialLeftForeheadInner = 0x843,
        FacialLeftForeheadInnerBulge = 0x767C,
        FacialLeftForeheadOuter = 0x8DCB,
        FacialSkull = 0x4221,
        FacialForeheadUpper = 0xF7D6,
        FacialLeftForeheadUpperInner = 0xCF13,
        FacialLeftForeheadUpperOuter = 0x509B,
        FacialRightForeheadUpperInner = 0xCEF3,
        FacialRightForeheadUpperOuter = 0x507B,
        FacialLefttemple = 0xAF79,
        FacialLeftEar = 0x19DD,
        FacialLeftEarLower = 0x6031,
        FacialLeftmasseter = 0x2810,
        FacialLeftJawRecess = 0x9C7A,
        FacialLeftCheekOuterSkin = 0x14A5,
        FacialRightCheekLower = 0xF367,
        FacialRightCheekLowerBulge1 = 0x599B,
        FacialRightCheekLowerBulge2 = 0x599C,
        FacialRightmasseter = 0x810,
        FacialRightJawRecess = 0x93D4,
        FacialRightEar = 0x1137,
        FacialRightEarLower = 0x8031,
        FacialRightEyesackLower = 0x777B,
        FacialRightNasolabialBulge = 0xD61E,
        FacialRightCheekOuter = 0xD32,
        FacialRightCheekInner = 0x737C,
        FacialRightNoseUpper = 0x1CD6,
        FacialRightForeheadInner = 0xE43,
        FacialRightForeheadInnerBulge = 0x769C,
        FacialRightForeheadOuter = 0x8FCB,
        FacialRightCheekOuterSkin = 0xB334,
        FacialRightEyesackUpperInnerFurrow = 0x9FAE,
        FacialRightEyesackUpperOuterFurrow = 0x140F,
        FacialRightEyesackUpperInnerBulge = 0xA359,
        FacialRightEyesackUpperOuterBulge = 0x1AF9,
        FacialRightNasolabialFurrow = 0x2CAA,
        FacialRightTemple = 0xAF19,
        FacialRightEyeball = 0x1944,
        FacialRightEyelidUpper = 0x7E14,
        FacialRightEyelidUpperOuterSDK = 0xB115,
        FacialRightEyelidUpperOuterAnalog = 0xF25A,
        FacialRightEyelashUpperOuter = 0xE0A,
        FacialRightEyelidUpperInnerSDK = 0xD541,
        FacialRightEyelidUpperInnerAnalog = 0x7C63,
        FacialRightEyelashUpperInner = 0x8172,
        FacialRightEyelidLower = 0x7FDF,
        FacialRightEyelidLowerOuterSDK = 0x1BD,
        FacialRightEyelidLowerOuterAnalog = 0x457B,
        FacialRightEyelashLowerOuter = 0xBE49,
        FacialRightEyelidLowerInnerSDK = 0xF351,
        FacialRightEyelidLowerInnerAnalog = 0xE13,
        FacialRightEyelashLowerInner = 0x3322,
        FacialLeftLipUpperSDK = 0x8F30,
        FacialLeftLipUpperAnalog = 0xB1CF,
        FacialLeftLipUpperThicknessH = 0x37CE,
        FacialLeftLipUpperThicknessV = 0x38BC,
        FacialLipUpperSDK = 0x1774,
        FacialLipUpperAnalog = 0xE064,
        FacialLipUpperThicknessH = 0x7993,
        FacialLipUpperThicknessV = 0x7981,
        FacialLeftLipCornerSDK = 0xB1C,
        FacialLeftLipCornerAnalog = 0xE568,
        FacialLeftLipCornerThicknessUpper = 0x7BC,
        FacialLeftLipCornerThicknessLower = 0xDD42,
        FacialRightLipUpperSDK = 0x7583,
        FacialRightLipUpperAnalog = 0x51CF,
        FacialRightLipUpperThicknessH = 0x382E,
        FacialRightLipUpperThicknessV = 0x385C,
        FacialRightLipCornerSDK = 0xB3C,
        FacialRightLipCornerAnalog = 0xEE0E,
        FacialRightLipCornerThicknessUpper = 0x54C3,
        FacialRightLipCornerThicknessLower = 0x2BBA,
        MHMulletRoot = 0x3E73,
        MHMulletScaler = 0xA1C2,
        MHHairScale = 0xC664,
        MHHairCrown = 0x1675,
        SMTorch = 0x8D6,
        FXLight = 0x8959,
        FXLightScale = 0x5038,
        FXLightSwitch = 0xE18E,
        BagRoot = 0xAD09,
        BagPivotRoot = 0xB836,
        BagPivot = 0x4D11,
        BagBody = 0xAB6D,
        BagBoneRight = 0x937,
        BagBoneLeft = 0x991,
        SMLifeSaverFront = 0x9420,
        SMRightPouchesRoot = 0x2962,
        SMRightPouches = 0x4141,
        SMLeftPouchesRoot = 0x2A02,
        SMLeftPouches = 0x4B41,
        SMSuitBackFlapper = 0xDA2D,
        SPRCopRadio = 0x8245,
        SMLifeSaverBack = 0x2127,
        MHBlushSlider = 0xA0CE,
        SkelTail01 = 0x347,
        SkelTail02 = 0x348,
        MHLeftConcertinaB = 0xC988,
        MHLeftConcertinaA = 0xC987,
        MHRightConcertinaB = 0xC8E8,
        MHRightConcertinaA = 0xC8E7,
        MHLeftShoulderBladeRoot = 0x8711,
        MHLeftShoulderBlade = 0x4EAF,
        MHRightShoulderBladeRoot = 0x3A0A,
        MHRightShoulderBlade = 0x54AF,
        FBRightEar000 = 0x6CDF,
        SPRRightEar = 0x63B6,
        FBLeftEar000 = 0x6439,
        SPRLeftEar = 0x5B10,
        FBTongueA000 = 0x4206,
        FBTongueB000 = 0x4207,
        FBTongueC000 = 0x4208,
        SkelLeftToe1 = 0x1D6B,
        SkelRightToe1 = 0xB23F,
        SkelTail03 = 0x349,
        SkelTail04 = 0x34A,
        SkelTail05 = 0x34B,
        SPRGonadsRoot = 0xBFDE,
        SPRGonads = 0x1C00,
        FBLeftBrowOut001 = 0xE3DB,
        FBLeftLidUpper001 = 0xB2B6,
        FBLeftEye001 = 0x62AC,
        FBLeftCheekBone001 = 0x542E,
        FBLeftLipCorner001 = 0x74AC,
        FBRightLidUpper001 = 0xAA10,
        FBRightEye001 = 0x6B52,
        FBRightCheekBone001 = 0x4B88,
        FBRightBrowOut001 = 0x54C,
        FBRightLipCorner001 = 0x2BA6,
        FBBrowCentre001 = 0x9149,
        FBUpperLipRoot001 = 0x4ED2,
        FBUpperLip001 = 0xF18F,
        FBLeftLipTop001 = 0x4F37,
        FBRightLipTop001 = 0x4537,
        FBJaw001 = 0xB4A0,
        FBLowerLipRoot001 = 0x4324,
        FBLowerLip001 = 0x508F,
        FBLeftLipBot001 = 0xB93B,
        FBRightLipBot001 = 0xC33B,
        FBTongue001 = 0xB987,
    }
}