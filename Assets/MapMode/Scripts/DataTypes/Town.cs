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
        public string townDescription;

        public IDictionary<string, float> supplies; 
        public IDictionary<string, int> predictedSupplies;
        public IDictionary<string, int> demand;
        public Dictionary<int, (int,float,string)> incomingFleets;//key, amount of goods, time to arrive, good coming
        public List<Fleet> fleets;

    }

    [TextArea(3,10)]
    public string DebugText = "";

    public Sprite[] setupSupplyIcons;
    string[] setupSupplyItems = new string[10] {"fish","lumber","fur","guns","sugar","coffee","salt","tea","tobacco","cotton" };
    public string[] setupSupplyDescription = new string[10] { "this is fish", " this is lumber", "this is fur", "this is guns", "this is sugar", "this is coffee", "this is salt", "this is tea", "this is tobacco", "this is cotton" };


    public Sprite townIcon;
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
    public string townDescription;


    TownManager townManager;
    IDictionary<string, float> supplies = new Dictionary<string, float>();
    IDictionary<string, int> predictedSupplies = new Dictionary<string, int>();
    IDictionary<string, int> demand = new Dictionary<string, int>();

    
    Dictionary<int, (int,float,string)> incomingFleets = new Dictionary<int, (int, float,string)>();//incoming fleet id, amount its carrying, expected time till it arrives
    public List<Fleet> dockedFleets = new List<Fleet>();

    private TownInfoUI townUI;

    #endregion

    #region Monobehaviour

    private void Awake()
    {
        townUI = GameObject.Find("TownOverview").GetComponent<TownInfoUI>();
    }
    void Start()
    {
        GameEvents.SaveInitiated += Save;// add events to happen when save and load is called
        GameEvents.LoadInitiated += Load;
        townManager = GetComponentInParent<TownManager>();
        

        productionAmount = new float[10]; // Ensure the array has at least 10 elements
        for (int i = 0; i < 10; i++)
        {
            productionAmount[i] = Random.Range(0.5f, 1.5f);
        }

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

    private void OnMouseOver()
    {
        townUI.DisplayTownUI(gameObject.GetComponent<Town>());
    }
    private void OnMouseExit()
    {
        townUI.CloseTownUI();
    }


    public void SendOutFleet(Fleet fleet,Transform destination) {
        Transform parent = GameObject.Find("/Boats").transform;
        GameObject fleetPrefab = Instantiate(prefabFleet,transform.position,Quaternion.identity,parent);
        fleetPrefab.GetComponent<FleetMapController>().SetFleet(fleet);
        fleetPrefab.GetComponent<FleetMapController>().destination = destination;

    }

    public Fleet MakeTradeFleet(string resource, int amount)
    {

        int numberOfBoats = Mathf.CeilToInt((float)amount / 100);

        Fleet f1 = new Fleet(nationality, "Trader");
        for (int i = 0; i < numberOfBoats; i++)
        {
            f1.AddBoat(new Boat(name+"HMS V" + i, BoatType.TradeShip));
        }
        FillCargo(f1, resource, amount);
        //Debug.Log("Boats in new trade fleet:" + f1.boats.Count);

        return f1;
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
            //can implment distance and cost to see if its wroth while in this function
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

        incomingFleets.Add(f, (amount, 200, item));//default 200. could change based on distance to travel 
        Debug.Log(gameObject.name + "Requested new shipment of " + item + " predicted sup:" + predictedSupplies[item]);

        while (incomingFleets.ContainsKey(f) && 0 < incomingFleets[f].Item2) { //if fleet arrives or time runs out end while loop
            incomingFleets[f] = (incomingFleets[f].Item1, incomingFleets[f].Item2 - Time.deltaTime, incomingFleets[f].Item3);//subtract off time 
            //Debug.Log(incomingFleets[f]); //could use varibale and subtract off 1 every second 
            yield return null;
        }

        if (incomingFleets.ContainsKey(f)) {
            predictedSupplies[item] -= amount;
            incomingFleets.Remove(f);
            Debug.Log(gameObject.name + incomingFleets.Count + "Requested shipment never arrived, predicted sup:" + predictedSupplies[item]);
        }
        
    }
    IEnumerator RestartExpected(int f){  
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
            price[i] = CalculateDynamicPrice(setupSupplyItems[i]);
        }
        return (setupSupplyItems,sup, dem, price);
    }

    public int SupplyOfItem(string item) {
        return (int)supplies[item]+predictedSupplies[item];
    }
    public int DemandOfItem(string item){
        return demand[item];
    }

    public float CalculateDynamicPrice(string item)
    {
        // Get supply, demand, and standard price for the item
        float supply = supplies.ContainsKey(item) ? (int)supplies[item] : 0f; // Cast supply to int as in your original code
        float demand = this.demand.ContainsKey(item) ? this.demand[item] : 0f;
        float standardPrice = townManager.standardPrices.ContainsKey(item) ? townManager.standardPrices[item] : 1f; // Fallback to 1f if missing

        const float minValue = 0.01f;
        supply = Mathf.Max(supply, minValue);
        demand = Mathf.Max(demand, minValue);

        float ratio = supply / demand;
        float multiplier;

        if (ratio <= 1)
        {
            // Interpolate between 4x (at ratio 0) and 1x (at ratio 1)
            multiplier = Mathf.Lerp(4f, 1f, ratio / 1f);
        }
        else if (ratio <= 2)
        {
            // Interpolate between 1x (at ratio 1) and 0.5x (at ratio 2)
            multiplier = Mathf.Lerp(1f, 0.5f, (ratio - 1f) / 1f);
        }
        else if (ratio <= 3)
        {
            // Interpolate between 0.5x (at ratio 2) and 0.25x (at ratio 3)
            multiplier = Mathf.Lerp(0.5f, 0.25f, (ratio - 2f) / 1f);
        }
        else
        {
            // Beyond 3x supply, cap the multiplier at 0.25x
            multiplier = 0.25f;
        }

        // Calculate final price
        return standardPrice * multiplier;
    }

    public float CalculateTransactionPrice(string item, int amount)
    {
        // Retrieve the current supply and demand for the item
        float supply = supplies.ContainsKey(item) ? (int)supplies[item] : 0f;
        float initialPrice = CalculateDynamicPrice(item); // Price before transaction

        // Adjust supply based on the amount being bought or sold
        supplies[item] = supply + amount;

        // Calculate price after the supply adjustment
        float adjustedPrice = CalculateDynamicPrice(item);

        // Restore original supply (so we don't actually modify the supply in this calculation)
        supplies[item] = supply;

        // Calculate the average price for the transaction
        float averagePrice = (initialPrice + adjustedPrice) / 2;

        // Total cost/revenue for the transaction
        return averagePrice * amount;
    }

    public bool FillCargoPlayer(Fleet fle, string resource,int amount) {
        if (PlayerGlobal.money < CalculateTransactionPrice( resource, amount)) {
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
            consumptionAmount[i] -= (float)demand[setupSupplyItems[i]] /1000;
        }
    }

    private void RunProduction() {
        for (int i = 0; i < productionAmount.Length; i++ ) {
            float production =  -consumptionAmount[i] * productionAmount[i] * Mathf.Pow(2f, -(supplies[setupSupplyItems[i]] / demand[setupSupplyItems[i]]) + 1);
            float consumption =  consumptionAmount[i];
            if (supplies[setupSupplyItems[i]] + production + consumption> 0) {
                supplies[setupSupplyItems[i]] += production;
                supplies[setupSupplyItems[i]] += consumption;
                //Debug.Log("Info:Town:Production:"+gameObject.name + setupSupplyItems[i] + consumptionAmount[i] + " " + production);
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
                + CalculateDynamicPrice(item.Key) + "\n";
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
        TownData data = new()
        {
            productionAmount = productionAmount,
            consumptionAmount = consumptionAmount,
            nationality = nationality,
            supplies = supplies,
            predictedSupplies = predictedSupplies,
            demand = demand,
            incomingFleets = incomingFleets,
            fleets = dockedFleets
        };
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
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        GameEvents.SaveInitiated -= Save;
        GameEvents.LoadInitiated -= Load;
    }
    #endregion
}
