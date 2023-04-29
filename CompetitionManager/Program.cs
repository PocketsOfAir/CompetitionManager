// See https://aka.ms/new-console-template for more information

using CompetitionManager.MatchupEngine;

var codeFirstTeams = new List<Team>
{
    new Team("SO SAD!", 1192),
    new Team("Frisbabes", 1230),
    new Team("Hexagons are the Bestagons", 1206),
    new Team("Fruit Ninjas", 1100),
    new Team("If it ain't true, it's Falseong", 1112),
    new Team("Nunchuckers", 1170),
    new Team("Radical Roadkill", 1172),
    new Team("UC Buckets", 1054),
    new Team("Zombicorns", 1036),
    new Team("Secret Agents", 1062),
    new Team("Discplaced", 992),
    new Team("Down To Huck", 978),
    new Team("Huckaholics Anonymous", 1020),
    new Team("Reverse Kelly Red", 881),
    new Team("Thank Huck Dyls Gone", 1019),
    new Team("Throws Before Hoes", 1109),
    new Team("To the Pub!", 974),
    new Team("Turducken", 1019),
    new Team("Frozen Fishes", 866),
    new Team("Redisculous", 907),
    new Team("Sports!", 928),
    new Team("Thor's Hammer", 960),
    new Team("Veni Vidi Vrisbee", 874),
    new Team("Where's Rosie?", 946),
    new Team("Huck, Huck Goose", 776),
    new Team("Bunch of Cuts", 724),
};


var codeFirstRounds = new List<CompletedRound>
{
    new CompletedRound()
    {
        Matches = new List<CompletedMatch>
        {
            new CompletedMatch("Zombicorns",8,"Secret Agents",15),
            new CompletedMatch("Throws Before Hoes",14,"Redisculous",11),
            new CompletedMatch("Sports!",15,"Huck, Huck Goose",3),
            new CompletedMatch("Thor's Hammer",15,"Bunch of Cuts",9),
            new CompletedMatch("Veni Vidi Vrisbee",6,"Where's Rosie?",13),
            new CompletedMatch("SO SAD!",13,"Nunchuckers",13),
            new CompletedMatch("Hexagons are the Bestagons",15,"If it ain't true, it's Falseong",9),
            new CompletedMatch("Fruit Ninjas",13,"Discplaced",6),
            new CompletedMatch("To the Pub!",9,"UC Buckets",13),
            new CompletedMatch("Down To Huck",11,"Huckaholics Anonymous",8),
            new CompletedMatch("Reverse Kelly Red",7,"Turducken",15),
            new CompletedMatch("Frisbabes",13,"Radical Roadkill",12),
            new CompletedMatch("Thank Huck Dyls Gone",14,"Frozen Fishes",6),
        }
    },
    new CompletedRound()
    {
        Matches = new List<CompletedMatch>
        {
            new CompletedMatch("Zombicorns",6,"If it ain't true, it's Falseong",15),
            new CompletedMatch("Frozen Fishes",10,"Veni Vidi Vrisbee",10),
            new CompletedMatch("Redisculous",6,"Sports!",15),
            new CompletedMatch("Thor's Hammer",9,"Where's Rosie?",7),
            new CompletedMatch("Huck, Huck Goose",9,"Bunch of Cuts",7),
            new CompletedMatch("Nunchuckers",15,"Secret Agents",0),
            new CompletedMatch("Radical Roadkill",14,"UC Buckets",5),
            new CompletedMatch("Fruit Ninjas",3,"SO SAD!",15),
            new CompletedMatch("Discplaced",11,"To the Pub!",11),
            new CompletedMatch("Down To Huck",9,"Turducken",11),
            new CompletedMatch("Huckaholics Anonymous",15,"Reverse Kelly Red",8),
            new CompletedMatch("Frisbabes",1,"Hexagons are the Bestagons",0),
            new CompletedMatch("Thank Huck Dyls Gone",7,"Throws Before Hoes",14),
        }
    },
    new CompletedRound()
    {
        Matches = new List<CompletedMatch>
        {
            new CompletedMatch("Zombicorns",13,"UC Buckets",11),
            new CompletedMatch("Frozen Fishes",15,"Bunch of Cuts",5),
            new CompletedMatch("Redisculous",1,"Veni Vidi Vrisbee",0),
            new CompletedMatch("Sports!",10,"Thor's Hammer",13),
            new CompletedMatch("Huck, Huck Goose",7,"Where's Rosie?",12),
            new CompletedMatch("Hexagons are the Bestagons",15,"Nunchuckers",6),
            new CompletedMatch("If it ain't true, it's Falseong",6,"Radical Roadkill",15),
            new CompletedMatch("Fruit Ninjas",15,"Secret Agents",0),
            new CompletedMatch("Discplaced",9,"Down To Huck",9),
            new CompletedMatch("Huckaholics Anonymous",15,"To the Pub!",5),
            new CompletedMatch("Reverse Kelly Red",5,"Thank Huck Dyls Gone",15),
            new CompletedMatch("Frisbabes",11,"SO SAD!",12),
            new CompletedMatch("Throws Before Hoes",11,"Turducken",6),
        }
    },
    new CompletedRound()
    {
        Matches = new List<CompletedMatch>
        {
            new CompletedMatch("Zombicorns",5,"Throws Before Hoes",11),
            new CompletedMatch("To the Pub!",10,"Thor's Hammer",8),
            new CompletedMatch("Frozen Fishes",15,"Huck, Huck Goose",4),
            new CompletedMatch("Sports!",12,"Where's Rosie?",13),
            new CompletedMatch("Veni Vidi Vrisbee",15,"Bunch of Cuts",5),
            new CompletedMatch("SO SAD!",12,"Hexagons are the Bestagons",12),
            new CompletedMatch("If it ain't true, it's Falseong",15,"Secret Agents",8),
            new CompletedMatch("Fruit Ninjas",4,"Radical Roadkill",15),
            new CompletedMatch("UC Buckets",13,"Turducken",7),
            new CompletedMatch("Discplaced",7,"Huckaholics Anonymous",14),
            new CompletedMatch("Down To Huck",8,"Thank Huck Dyls Gone",13),
            new CompletedMatch("Frisbabes",15,"Nunchuckers",6),
            new CompletedMatch("Reverse Kelly Red",9,"Redisculous",14),
        }
    },
};

var matchupGenerator = new MatchupGenerator(11, codeFirstTeams, codeFirstRounds);
matchupGenerator.GenerateRound();
