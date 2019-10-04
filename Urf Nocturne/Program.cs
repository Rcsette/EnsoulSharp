namespace URFNocturne
{
    using System;
    using System.Linq;
    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using EnsoulSharp.SDK.MenuUI.Values;
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

        public static int[] rRanges = new int[] { 2500, 3250, 4000 };
        
        public static void OnGameLoad()
        {
            
            if (ObjectManager.Player.CharacterName != "Nocturne")
            {
                return;
            }

            CreateEvents();

            Q = new Spell(SpellSlot.Q, 1200f);
	    

            W = new Spell(SpellSlot.W, 400f);
	   

            E = new Spell(SpellSlot.E, 425f);
	    

            R = new Spell(SpellSlot.R, rRanges[0]);
	
	        Q.SetSkillshot(0.25f, 60f, 1300, false, false, SkillshotType.Line);
 	    
 
            MainMenu = new Menu("URF Nocturne", "URF Nocturne", true);


            var comboMenu = new Menu("Combo", "Combo Config");
            comboMenu.Add(new MenuBool("comboQ", "Use Q", true));
            comboMenu.Add(new MenuBool("comboW", "Use W", true));
            comboMenu.Add(new MenuBool("comboE", "Use E", true));
            comboMenu.Add(new MenuBool("comboR", "Use R", true));
            comboMenu.Add(new MenuSlider("EHR", "Enemy HP Use [R] <=", 15));
            MainMenu.Add(comboMenu);

            var harassMenu = new Menu("Harass", "Harass Config");
            harassMenu.Add(new MenuBool("harassQ", "Use Q", true));
            harassMenu.Add(new MenuBool("harassW", "Use W", true));
            harassMenu.Add(new MenuBool("harassE", "Use W", true));
            MainMenu.Add(harassMenu);

            var laneclearMenu = new Menu("Clear", "Lane Clear");
	        laneclearMenu.Add(new MenuBool("clearQ", "Use Q", true));
            laneclearMenu.Add(new MenuBool("clearE", "Use E", true));
            MainMenu.Add(laneclearMenu);

            var jungleclearMenu = new Menu("JClear", "Jungle Clear");
            jungleclearMenu.Add(new MenuBool("jclearQ", "Use Q", true));
            jungleclearMenu.Add(new MenuBool("jclearE", "Use E", true));
            MainMenu.Add(jungleclearMenu);

            var drawMenu = new Menu("Draw", "Draw Spells");
            drawMenu.Add(new MenuBool("drawQ", "Draw Q", true));
            drawMenu.Add(new MenuBool("drawE", "Draw E", true));
            drawMenu.Add(new MenuBool("drawR", "Draw R", true));
            MainMenu.Add(drawMenu);


            MainMenu.Attach();

           
        }

        private static void CreateEvents()
        {
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
        }
         private static void DrawingOnOnDraw(EventArgs args)
        {
            if (MainMenu["draw"]["drawQ"].GetValue<MenuBool>().Enabled)

                Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Green);

            if (MainMenu["draw"]["drawE"].GetValue<MenuBool>().Enabled)

                Render.Circle.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Green);

            if (MainMenu["draw"]["drawR"].GetValue<MenuBool>().Enabled)

                Render.Circle.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Green);
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
                               
                {

                    
                    W.Cast();
                    
                }

            }

            if (MainMenu["Combo"]["comboE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(E.Range, DamageType.Physical);


                if (target != null && target.IsValidTarget(E.Range))
                {

                    E.CastOnUnit(target);
                }
            }


            if (MainMenu["Combo"]["comboR"].GetValue<MenuBool>().Enabled && R.IsReady() && ObjectManager.Player.CountEnemyHeroesInRange(R.Range) >= 1)
            {
                AIHeroClient target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                

                if (target != null && target.IsValidTarget(R.Range))
		        {
                    if (target.Health <= R.GetDamage(target))
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

            if (MainMenu["Harass"]["harassE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {

                AIHeroClient target = TargetSelector.GetTarget(E.Range, DamageType.Physical);


                if (target != null && target.IsValidTarget(E.Range))
                {

                    E.CastOnUnit(target);
                }

            }


     }

        private static void LaneClear()
        {
            var qminions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).Cast<AIBaseClient>().ToList();
            var eminions = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(E.Range)).Cast<AIBaseClient>().ToList();


            var qminionLocation = Q.GetCircularFarmLocation(qminions);
            


            if (MainMenu["Clear"]["clearQ"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                if (qminions.Any())
                { 
                    Q.Cast(qminionLocation.Position);
                }
                    
            }
		    if (MainMenu["Clear"]["clearE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {

                if (eminions.Any())
                {
                    E.Cast();
                } 
            }

        }

        private static void JungleClear()
        {
            var qmobs = GameObjects.Jungle.Where(x => x.IsValidTarget(Q.Range) && x.GetJungleType() != JungleType.Unknown).ToList();
            var emobs = GameObjects.Jungle.Where(x => x.IsValidTarget(E.Range) && x.GetJungleType() != JungleType.Unknown).ToList();

            var qfarm = Q.GetCircularFarmLocation(qmobs);
            var efarm = E.GetCircularFarmLocation(emobs);




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
            if (MainMenu["JClear"]["jclearE"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                if (emobs.Any())
                {
                    if (efarm.MinionsHit >= 1 || emobs.Any(x => x.GetJungleType() != JungleType.Small) && efarm.MinionsHit >= 1)
                    {
                        E.Cast();
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

