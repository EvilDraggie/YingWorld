﻿using System;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using System.IO;

namespace ShellTooth
{
	public class ThinkNode_ChancePerHour_Humpin : ThinkNode_ChancePerHour
	{
		protected override float MtbHours(Pawn pawn)
		{
			if (pawn.CurrentBed() == null)
			{
				return -1f;
			}
			Pawn partnerInMyBed = LovePartnerRelationUtility.GetPartnerInMyBed(pawn);
			if (partnerInMyBed == null)
			{
				return -1f;
			}
			float humpMTB = GetHumpinMtbHours(pawn, partnerInMyBed);
			float humpChance = (humpMTB != 0 && humpMTB < 60) ? GetHumpinMtbHours(pawn, partnerInMyBed) : 70;
			return humpChance;
		}
		public static float GetHumpinMtbHours(Pawn pawn, Pawn partner)
		{
			if (pawn.Dead || partner.Dead)
			{
				return -1f;
			}
			if (DebugSettings.alwaysDoLovin)
			{
				return 0.1f;
			}
			if (pawn.needs.food.Starving || partner.needs.food.Starving)
			{
				return -1f;
			}
			if (pawn.health.hediffSet.BleedRateTotal > 0f || partner.health.hediffSet.BleedRateTotal > 0f)
			{
				return -1f;
			}
			float num = HumpinMtbSinglePawnFactor(pawn);
			if (num <= 0f)
			{
				return -1f;
			}
			float num2 = HumpinMtbSinglePawnFactor(partner);
			if (num2 <= 0f)
			{
				return -1f;
			}
			float num3 = 6f;
			num3 *= num;
			num3 *= num2;
			num3 *= GenMath.LerpDouble(-100f, 100f, 1.3f, 0.7f, (float)pawn.relations.OpinionOf(partner));
			num3 *= GenMath.LerpDouble(-100f, 100f, 1.3f, 0.7f, (float)partner.relations.OpinionOf(pawn));
			if (pawn.health.hediffSet.HasHediff(HediffDefOf.PsychicLove, false))
			{
				num3 /= 4f;
			}
			Log.Message($"Humpin MTB between {pawn} and {partner} was {num3}.");
			return num3;
		}
		private static float HumpinMtbSinglePawnFactor(Pawn pawn)
		{
			float num = 1f;
			num /= 1f - pawn.health.hediffSet.PainTotal;
			float level = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);
			if (level < 0.5f)
			{
				num /= level * 2f;
			}
			return num / GenMath.FlatHill(0f, 0f, 2f, 12f, 20f, 0.2f, pawn.ageTracker.AgeBiologicalYearsFloat);
		}
	}
}