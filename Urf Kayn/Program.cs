namespace URFKayn
{
    using System;
    using System.Linq;
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
    using EnsoulSharp.SDK.Events;
    using EnsoulSharp.SDK.Utility;
    using EnsoulSharp.SDK.Prediction;
    using Color = System.Drawing.Color;
    using Menu = EnsoulSharp.SDK.MenuUI.Menu;
   

    public class Program
    {
        private static Menu MainMenu;

        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        public static void OnGameLoad()
        {
            
            if (ObjectManager.Player.CharacterName != "Kayn")
            {
                return;
            }

            CreateEvents();

            Q = new Spell(SpellSlot.Q, 600f);
	    

            W = new Spell(SpellSlot.W, 700f);
	   

            E = new Spell(SpellSlot.E, 700f);
	    

            R = new Spell(SpellSlot.R, 650f);
	
	    Q.SetSkillshot(0.25f, 50f, float.MaxValue, false, false, SkillshotType.Line);
 	    W.SetSkillshot(0.5f, 90f, 2500, false, false, SkillshotType.Line);
 
            MainMenu = new Menu("URF Kayn", "URF Kayn", true);

            
            var comboMenu = new Menu("Combo", "Combo Config");
            comboMenu.Add(new MenuBool("comboQ", "Use Q", true));
            comboMenu.Add(new MenuBool("comboW", "Use W", true));
            comboMenu.Add(new MenuBool("comboE", "Use E", true));
            comboMenu.Add(new MenuBool("comboR", "Use R", true));
            comboMenu.Add(new MenuSlider("MHR", "My HP Use [R] <=", 15));
            comboMenu.Add(new MenuSlider("EHR", "Enemy HP Use [R] <=", 15));
            MainMenu.Add(comboMenu);

	    var harassMenu = new Menu("Harass", "Harass Config");
            harassMenu.Add(new MenuBool("harassQ", "Use Q", true));
            harassMenu.Add(new MenuBool("harassW", "Use W", true));
            MainMenu.Add(harassMenu);

            var laneclearMenu = new Menu("Clear", "Lane Clear");
	    laneclearMenu.Add(new MenuBool("clearQ", "Use Q", true));
            laneclearMenu.Add(new MenuBool("clearW", "Use W", true));
            MainMenu.Add(laneclearMenu);

             var jungleclearMenu = new Menu("JClear", "Jungle Clear");
            jungleclearMenu.Add(new MenuBool("jclearQ", "Use Q", true));
            jungleclearMenu.Add(new MenuBool("jclearW", "Use W", true));
            MainMenu.Add(jungleclearMenu);
            


            MainMenu.Attach();

           
        }

        private static void CreateEvents()
        {
            Tick.OnTick += OnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
        }
         private static void DrawingOnOnDraw(EventArgs args)
        {
            
                Render.Circle.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Green);
            

        }



            private static void Combo()
        {
            if (MainMenu["Combo"]["comboQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {

                AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);


                if (target != null && target.IsValidTarget(Q.Range))
                {


                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition, true);
                    }
                }

            }

            if (MainMenu["Combo"]["comboW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {

                AIHeroClient target = TargetSelector.GetTarget(W.Range);
                

                if (target != null && target.IsValidTarget(W.Range))
                {

                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                    W.Cast(pred.CastPosition, true);
                    }
                }

            }


            
            if (MainMenu["Combo"]["comboR"].GetValue<MenuBool>().Enabled && R.IsReady() && ObjectManager.Player.CountEnemyHeroesInRange(750) >= 1)
            {
                AIHeroClient target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                

                if (target != null && target.IsValidTarget(R.Range))
		        {
                    if (target.Health <= R.GetDamage(target))
                    {
                        R.CastOnUnit(target);
                    }

                    else if (ObjectManager.Player.HealthPercent <= MainMenu["Combo"]["MHR"].GetValue<MenuSlider>().Value)
                    {
                        R.CastOnUnit(target);
                    }

                    else if (target.HealthPercent <= MainMenu["Combo"]["EHR"].GetValue<MenuSlider>().Value)
                    {
                        R.CastOnUnit(target);
                    }
                }
            }
           
        }

	private static void Harass()
        {
            if (MainMenu["Harass"]["harassQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {

                AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);


                if (target != null && target.IsValidTarget(Q.Range))
                {

                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                        Q.Cast(pred.CastPosition, true);
                    }
                }
            }

            if (MainMenu["Harass"]["harassW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {

                AIHeroClient target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                

                if (target != null && target.IsValidTarget(W.Range))
                {
		            var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= HitChance.Medium)
                    {
                          W.Cast(pred.CastPosition, true);
                    }
                }

            }


     }

        private static void LaneClear()
        {
            var qminions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).Cast<AIBaseClient>().ToList();
            var wminions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(W.Range)).Cast<AIBaseClient>().ToList();


            var qminionLocation = Q.GetCircularFarmLocation(qminions);
            var wminionLocation = W.GetCircularFarmLocation(wminions);


            if (MainMenu["Clear"]["clearQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                if (qminions.Any())
                { 
                    Q.Cast(qminionLocation.Position);
                }
                    
            }
		    if (MainMenu["Clear"]["clearW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {

                if (wminions.Any())
                {
                    W.Cast(wminionLocation.Position);
                } 
            }

        }

        private static void JungleClear()
        {
            var qmobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() != JungleType.Unknown).ToList();
            var wmobs = GameObjects.Jungle.Where(x => x.IsValidTarget(W.Range) && x.GetJungleType() != JungleType.Unknown).ToList();

            var qfarm = Q.GetCircularFarmLocation(qmobs);
            var wfarm = W.GetCircularFarmLocation(wmobs);




            if (MainMenu["JClear"]["jclearQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
                {
                if (qmobs.Any())
                {
                    if (qfarm.MinionsHit >= 1 || qmobs.Any(x => x.GetJungleType() != JungleType.Small) && qfarm.MinionsHit >= 1)
                    {
                        Q.Cast(qfarm.Position);
                    }
                }
                }
            if (MainMenu["JClear"]["jclearW"].GetValue<MenuBool>().Enabled && W.IsReady())
            {
                if (wmobs.Any())
                {
                    if (wfarm.MinionsHit >= 1 || wmobs.Any(x => x.GetJungleType() != JungleType.Small) && wfarm.MinionsHit >= 1)
                    {
                        W.Cast(wfarm.Position);
                    }
                }
            }
        }
    




        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
 		        case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
             
            }
        }
      
    }
}

