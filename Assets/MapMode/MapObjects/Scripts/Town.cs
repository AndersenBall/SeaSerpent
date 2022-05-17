using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    #region variables

    [System.Serializable]
    public struct TownData
    {
        public float[] productionAmount;
        public float[] consumptionAmount;
        public string nationality;

        public IDictionary<string, float> supplies; 
        public IDictionary<string, int> predictedSupplies;
        public IDictionary<string, int> demand;
        public Dictionary<int, (int,float,string)> incomingFleets;//key, amount of goods, time to arrive, good coming
        public List<Fleet> fleets;

    }

    [TextArea(3,10)]
    public string DebugText = "";
    
    string[] setupSupplyItems = new string[10] {"fish","lumber","fur","guns","sugar","coffee","salt","tea","tobacco","cotton" };
    
    [SerializeField]
    int[] setupSupplyCount;
    [SerializeField]
    int[] setupDemandCount;
    [SerializeField]
    float[] productionAmount;
    [SerializeField]
    float[] consumptionAmount;
    
    float timerOne = 0;
    float timerTwo = 0;

    public GameObject prefabFleet;
    
    public string nationality;
    

    TownManager townManager;
    IDictionary<string, float> supplies = new Dictionary<string, float>();
    IDictionary<string, int> predictedSupplies = new Dictionary<string, int>();
    IDictionary<string, int> demand = new Dictionary<string, int>();

    
    Dictionary<int, (int,float,string)> incomingFleets = new Dictionary<int, (int, float,string)>();//incoming fleet id, amount its carrying, expected time till it arrives
    public List<Fleet> dockedFleets = new List<Fleet>();
    #endregion

    #region Monobehaviour
    void Start()
    {
        GameEvents.SaveInitiated += Save;// add events to happen when save and load is called
        GameEvents.LoadInitiated += Load;
        townManager = GetComponentInParent<TownManager>();
        
        
        for (int i = 0; i < 10; i++) {
            supplies.Add(setupSupplyItems[i], setupSupplyCount[i]);
        }
        for (int i = 0; i < 10; i++) {
            predictedSupplies.Add(setupSupplyItems[i], 0);
        }
        for (int i = 0; i < 10; i++) {
            demand.Add(setupSupplyItems[i], setupDemandCount[i]);
        }
        SetUpConsumption();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDebugText();
        timerOne += Time.deltaTime;
        timerTwo += Time.deltaTime;
        if (timerOne > 1) {
            RunProduction();
            //RequestGoods();
            timerOne = 0;
        }
        //Verify if the timer is greater than delay:
        if (timerTwo > 5) {
            RequestGoods();
            //SendOutFleet(MakeTradeFleet("sugar"), townManager.towns[1].transform);
            timerTwo = 0;
        }
    }
    #endregion

    #region Methods
    public void DockFleet(Fleet fleet) {
        if (fleet.commander != "Trader") {
            dockedFleets.Add(fleet);
        }
    }

    
    public void SendOutFleet(Fleet fleet,Transform destination) {
        Transform parent = GameObject.Find("/Boats").transform;
        GameObject fleetPrefab = Instantiate(prefabFleet,transform.position,Quaternion.identity,parent);
        fleetPrefab.GetComponent<FleetMapController>().SetFleet(fleet);
        fleetPrefab.GetComponent<FleetMapController>().destination = destination;
        fleetPrefab.GetComponent<Unit>().target = destination;
    }

    private void RequestGoods() {
        List<(float, string)> mostNeeded = MostNeededItems();// FIND LARGEST IF FAIL ON REQUEST GO TO NEXT
        int amountWant = 0;
        foreach ((float, string) itemToRequest in mostNeeded) {
            if(!ItemAlreadyRequested(itemToRequest)) { continue; }
            if (deficit(itemToRequest.Item2) < 50) { continue; }

            if (deficit(itemToRequest.Item2) < 801) {
                amountWant = deficit(itemToRequest.Item2);
            }
            else
                amountWant = 800;

            (Fleet, int) sentOutFleet = townManager.RequestItemNonSurplus(itemToRequest.Item2, amountWant, gameObject.GetComponent<Town>());

            if (sentOutFleet.Item2 > 0 && sentOutFleet.Item1 != null) {
                StartCoroutine(RunExpected(itemToRequest.Item2, sentOutFleet.Item2, sentOutFleet.Item1));
                return;
            }
        }

        bool ItemAlreadyRequested((float, string) itemToRequest)
        {
            foreach ((int,float,string) f in incomingFleets.Values) {
                if (f.Item3 == itemToRequest.Item2) {
                    //Debug.Log("item is already being shipped" + itemToRequest.Item2);
                    return false;
                }
            }
            return true;
        }
    }
 
    IEnumerator RunExpected(string item, int amount, Fleet fle){
        int f = fle.FleetID;
        yield return new WaitForSeconds(1);
        predictedSupplies[item] += amount;

        incomingFleets.Add(f, (amount, 200, item));
        //Debug.Log(gameObject.name + "Requested new shipment of " + item + " predicted sup:" + predictedSupplies[item]);

        while (incomingFleets.ContainsKey(f) && 0 < incomingFleets[f].Item2) {
            incomingFleets[f] = (incomingFleets[f].Item1, incomingFleets[f].Item2 - Time.deltaTime, incomingFleets[f].Item3);
            
            //Debug.Log(incomingFleets[f]);
            yield return null;
        }

        if (incomingFleets.ContainsKey(f)) {
            predictedSupplies[item] -= amount;
            incomingFleets.Remove(f);
            Debug.Log(gameObject.name + incomingFleets.Count + "Requested shipment never arrived, predicted sup:" + predictedSupplies[item]);
        }
        
    }
    IEnumerator RestartExpected(int f)
    {
        
        int amount = incomingFleets[f].Item1;
        string item = incomingFleets[f].Item3;
        //Debug.Log(gameObject.name + "Requested new shipment of " + item + " predicted sup:" + predictedSupplies[item]);
        //Debug.Log(incomingFleets[f]);
        while (incomingFleets.ContainsKey(f) && 0 < incomingFleets[f].Item2) {//Item2 is timer
            incomingFleets[f] = (incomingFleets[f].Item1, incomingFleets[f].Item2 - Time.deltaTime, incomingFleets[f].Item3);
            
            yield return null;
        }

        if (incomingFleets.ContainsKey(f)) {
            predictedSupplies[item] -= amount;
            incomingFleets.Remove(f);
            Debug.Log(gameObject.name + incomingFleets.Count + "Requested shipment never arrived, predicted sup:" + predictedSupplies[item]);
        }

    }

    public Fleet MakeTradeFleet(string resource,int amount) {
   
        int numberOfBoats = Mathf.CeilToInt((float)amount / 100);
      
        Fleet f1 = new Fleet(nationality, "Trader");
        for (int i = 0; i < numberOfBoats; i++) {
            f1.AddBoat(new Boat("HMS V"+i, "TradeShip"));
        }
        FillCargo(f1, resource, amount);
        //Debug.Log("Boats in new trade fleet:" + f1.boats.Count);
        
        return f1;
    }
    
    public List<(float, string)> MostNeededItems(){//return list of most needed
        List<(float,string)> percentAmounts = new List<(float,string)>();

        foreach (KeyValuePair<string, float> supItem in supplies) {
            float percent = (float)(demand[supItem.Key] + 1) / ((int)supItem.Value + 1 + predictedSupplies[supItem.Key]);
            //Debug.Log(supItem.Key + " percent:" + percent + "demand:" + demand[supItem.Key] + "supplies:" + supItem.Value + "+" + predictedSupplies[supItem.Key]);
            if (percent > 1f) { 
                percentAmounts.Add((percent, supItem.Key));
            }        
        }
        percentAmounts.Sort();
        percentAmounts.Reverse();
        return percentAmounts;
    }

    public int Surplus(string item) {
        return (int)supplies[item] - (int)demand[item];
    }

    public int deficit(string item){
        return demand[item] - (int)supplies[item] - predictedSupplies[item];
    }
    public (string[],int[], int[], float[]) SupplyDemandPrice() {
        int[] sup = new int[10];
        int[] dem = new int[10];
        float[] price = new float[10];

        
        for (int i = 0; i < 10; i++) {
            sup[i] = (int)supplies[setupSupplyItems[i]];
        }
        for (int i = 0; i < 10; i++) {
            dem[i] = (int)demand[setupSupplyItems[i]];
        }
        for (int i = 0; i < 10; i++) {
            price[i] = CalculatePrice(townManager.standardPrices[setupSupplyItems[i]],setupSupplyItems[i]);
        }
        return (setupSupplyItems,sup, dem, price);
    }

    public int SupplyOfItem(string item) {
        return (int)supplies[item]+predictedSupplies[item];
    }
    public int DemandOfItem(string item){
        return demand[item];
    }
    public float CalculatePrice(float standardPrice, string item)
    {
        float sup = (int)supplies[item];
        float dem = demand[item];
        float price;
        if (sup < dem) {
            price = standardPrice * Mathf.Pow(4, -(sup / dem) + 1);
        }
        else {
            price = standardPrice * Mathf.Pow(2, -(sup / dem) + 1);
        }

        //Debug.Log(name + " " + item+ " calculated price " + price + " standard price " + standardPrice);
        return price;

    }

    public float CalculatePrice(float standardPrice, string item, int amountBought) {//town buying /ship sell
        float sup = (int)supplies[item];
        float sup2 = (int)supplies[item]+amountBought;
        float dem = demand[item];
        float price;
        float price2;
        if (sup < dem) {
            price = standardPrice * Mathf.Pow(4, -(sup / dem) + 1);
        }
        else {
            price = standardPrice * Mathf.Pow(2, -(sup / dem) + 1);
        }

        if (sup2 < dem) {
            price2 = standardPrice * Mathf.Pow(4, -(sup2 / dem) + 1);
        }
        else {
            price2 = standardPrice * Mathf.Pow(2, -(sup2 / dem) + 1);
        }

        //Debug.Log(name + " " + item+ " calculated prices " + price +","+ price2 + " standard price " + standardPrice+"amount buying:"+amountBought);
        return Mathf.Abs(amountBought*(price+price2)/2);
        
    }

    public bool FillCargoPlayer(Fleet fle, string resource,int amount) {
        if (PlayerGlobal.money < CalculatePrice(townManager.standardPrices[resource], resource, amount)) {
            Debug.Log("not enough money");
            return false;
        }

        int currentAddedCargo = 0;
        //Debug.Log(fle.commander +"ordered: "+ resource +","+ amount+" from:"+ name);
        if (!supplies.ContainsKey(resource)) {
            Debug.Log(gameObject.name + " Doesnt contain:" + resource);
        }

        foreach (Boat b in fle.GetBoats()) {
            int cargospace = b.GetCargoMax() - b.GetCargoCurrent();
            Debug.Log(b.boatName +" space on ship:"+ cargospace +" amount in town: " + supplies[resource]);
            if ((int)supplies[resource] - cargospace > 0) {
                if (currentAddedCargo + cargospace < amount) {
                    currentAddedCargo += cargospace;
                    b.AddCargo(resource, cargospace);
                    supplies[resource] -= cargospace;
                }
                else {

                    b.AddCargo(resource, amount - currentAddedCargo);
                    supplies[resource] -= amount - currentAddedCargo;
                    break;
                }
            }
            else {
                //Debug.Log("settlement doesnt have enough resource:" + resource +"added:" + supplies[resource]);
                b.AddCargo(resource, (int)supplies[resource]);
                supplies[resource] -= (int)supplies[resource];
            }
        }
        return true;
    }
    public void FillCargo(Fleet fle, string resource, int amount)
    {

        int currentAddedCargo = 0;
        //Debug.Log(fle.commander +"ordered: "+ resource +","+ amount+" from:"+ name);
        if (!supplies.ContainsKey(resource)) {
            Debug.Log(gameObject.name + " Doesnt contain:" + resource);
        }

        foreach (Boat b in fle.GetBoats()) {
            int cargospace = b.GetCargoMax() - b.GetCargoCurrent();
            Debug.Log(b.boatName + " space on ship:" + cargospace + " amount in town: " + supplies[resource]);
            if ((int)supplies[resource] - cargospace > 0) {
                if (currentAddedCargo + cargospace < amount) {
                    currentAddedCargo += cargospace;
                    b.AddCargo(resource, cargospace);
                    supplies[resource] -= cargospace;
                }
                else {

                    b.AddCargo(resource, amount - currentAddedCargo);
                    supplies[resource] -= amount - currentAddedCargo;
                    break;
                }
            }
            else {
                //Debug.Log("settlement doesnt have enough resource:" + resource +"added:" + supplies[resource]);
                b.AddCargo(resource, (int)supplies[resource]);
                supplies[resource] -= (int)supplies[resource];

            }
        }
    }
    public float SellItemsInCargo(Fleet fle,int amount,string resource){//script for boats
        int fleetID = fle.FleetID;
        string ite ="None";
        int amountSold = 0;
        foreach (Boat boat in fle.GetBoats()) {
            if (!HelperSellInBoat(resource,amount, ref ite, ref amountSold, boat)) { 
                break;
            }
        }
        if (ite != "None") {
            int amountRemoved = 0;
            if (incomingFleets.ContainsKey(fleetID)) {
                predictedSupplies[ite] -= incomingFleets[fleetID].Item1;//remove predicted incoming supplies 
                amountRemoved = incomingFleets[fleetID].Item1;
                incomingFleets.Remove(fleetID);
            }
            Debug.Log("fleet came in" + fle.commander + gameObject.name + " removed predicted:" + ite + amountRemoved);


        }
        return 1f;
    }

    private bool HelperSellInBoat(string resource, int amount, ref string ite, ref int amountSold, Boat boat)
    {
        List<(string s, int a)> toRemove = new List<(string, int)>();
        bool didntStop = true;
        foreach (KeyValuePair<string, int> item in boat.getSupplies()) {
            ite = item.Key;// to tell town what ai trade fleet carries
            if (resource == item.Key || resource == "All") {
                if (item.Value + amountSold < amount) {
                    supplies[item.Key] += item.Value;//add to town
                    toRemove.Add((item.Key, item.Value));
                    amountSold += item.Value;
                }
                else {
                    supplies[item.Key] += amount - amountSold;//add to town
                    toRemove.Add((item.Key, amount - amountSold));
                    amountSold += amount - amountSold;
                    didntStop = false;
                    break;
                }
                //Debug.Log(boat.boatName + " sold: " + item.Key + "," + item.Value + name +". New town amount:" + supplies[item.Key]);
            }
            
        }
        foreach ((string, int) tup in toRemove) {
            boat.RemoveCargo(tup.Item1, tup.Item2);
        }
        return didntStop;
    }

    private void SetUpConsumption(){
        for (int i = 0; i < 10; i++) {
            consumptionAmount[i] -= (float)demand[setupSupplyItems[i]] /730;
        }
    }
    private void RunProduction() {
        for (int i = 0; i < productionAmount.Length; i++ ) {
            float production =  -consumptionAmount[i] * productionAmount[i] * Mathf.Pow(2f, -(supplies[setupSupplyItems[i]] / demand[setupSupplyItems[i]]) + 1);
            float consumption =  consumptionAmount[i];
            if (supplies[setupSupplyItems[i]] + production + consumption> 0) {
                supplies[setupSupplyItems[i]] += production;
                supplies[setupSupplyItems[i]] += consumption;
                //Debug.Log(gameObject.name + (consumptionAmount[i] -consumptionAmount[i] * productionAmount[i] * Mathf.Pow(1.25f, -(supplies[setupSupplyItems[i]] / demand[setupSupplyItems[i]]) + 1)));
            }
            else { Debug.Log(gameObject.name + "error, not enough supply to consume." + setupSupplyItems[i]); }
        }
        UpdateDebugText();
    }
    
   
    
    #endregion
    #region Dev Methods
    private void UpdateDebugText(){
        DebugText = "Supplies:\n";
        int count = 0;
        foreach (KeyValuePair<string, float> item in supplies) {
            DebugText += count +": "+ item.Key + (int)item.Value + "+"+predictedSupplies[item.Key]+"->"+ (int)demand[item.Key] +" p:"
                + CalculatePrice(townManager.standardPrices[item.Key],item.Key) + "\n";
            count += 1;
        }
        
        DebugText += "\nFleets in Dock:";
        foreach (Fleet f in dockedFleets) {
            DebugText += f.commander + ",";
        }
        DebugText += "\nIncoming Fleet IDs:";
        foreach (int id in incomingFleets.Keys) {
            DebugText += id + ",";
        }
    }

    public void Save() {
        TownData data = new TownData();
        data.productionAmount = productionAmount;
        data.consumptionAmount = consumptionAmount;
        data.nationality = nationality;
        data.supplies = supplies;
        data.predictedSupplies = predictedSupplies;
        data.demand = demand;
        data.incomingFleets = incomingFleets;
        data.fleets = dockedFleets;
        SaveLoad.Save(data, name);

    }
    public void Load()
    {
        StopAllCoroutines();
        TownData data;
        if (SaveLoad.SaveExists(name)) {
            data = SaveLoad.Load<TownData>(name);
            productionAmount = data.productionAmount;
            consumptionAmount = data.consumptionAmount;
            nationality = data.nationality;
            supplies = data.supplies;
            predictedSupplies = data.predictedSupplies;
            demand = data.demand;
            incomingFleets = data.incomingFleets;
            dockedFleets = data.fleets;
            List<int> fleetList = new List<int>(incomingFleets.Keys);
            foreach (int id in fleetList) {
                StartCoroutine( RestartExpected(id));
            }
        }


    }
    #endregion
}
