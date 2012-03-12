var RulesProcessor = (function(){
    
    var self = {
        byteLocators: {
            BYTES_PER_PACKET:                    256,
            FLASH_MEMORY_POS:                    "00400000",

            OVERALL_START_MACRO_ENTRY:            "\x01",
            OVERALL_END_MACRO_ENTRY:            "\x02",

            START_MACRO_NAME:                    "\x03",
            END_MACRO_NAME:                        "\x04",

            START_ACTION_ENTRY:                    "\x05",
            END_ACTION_ENTRY:                    "\x06",

            START_ACTION_DATA:                    "\x07",
            END_ACTION_DATA:                    "\x08",

            OVERALL_START_RULE:                    "\x09",
            OVERALL_END_RULE:                    "\x0B",

            START_RULE_NAME: 	               "\x0C",
            END_RULE_NAME: 	                   "\x0E",

            START_SEARCH_STRING:                "\x0F",
            END_SEARCH_STRING:                    "\x10",

            START_MAPPED_MACRO_NAME:            "\x11",
            END_MAPPED_MACRO_NAME:                "\x12",

            FILLER_BYTE:                        "\xFF",
        },
    };

    self.getBytes = function (ruleCol) {
        var bytes = [];

		if (!ruleCol.macros) {
			// probably parsed in a string, not an object, so lets convert it to the object
		 	 ruleCol = JSON.parse(ruleCol);
		 	 if (!ruleCol.macros) {
		 	 	// Still failed to create a correct object, so give up
		 	 	console.log("RulesCollection was invalid");
		 	 	return;
		 	 }
		}

        // Number of macros, padded to 5 digits
        bytes.push(padZero(5, ruleCol["macros"].length));

        var macroBytes = [];
        var actionBytes = [];
        var theMacro, theAction;

        // Loop through each macro
        for (var i = 0; i < ruleCol.macros.length; i++) {
            theMacro = ruleCol.macros[i];

            bytes.push(self.byteLocators.OVERALL_START_MACRO_ENTRY);

			macroBytes = [];

            // Length of macro name
            macroBytes.push(padZero(5, theMacro.name.length));
            
            // Macro name
            macroBytes.push(self.byteLocators.START_MACRO_NAME);
            macroBytes.push(theMacro.name);
            macroBytes.push(self.byteLocators.END_MACRO_NAME);

            // Number of actions
            macroBytes.push(padZero(5, theMacro.actions.length));
            
            // Loop through each action
            for (var j = 0; j < theMacro.actions.length; j++) {
                theAction = theMacro.actions[j];
                actionBytes = [];

                macroBytes.push(self.byteLocators.START_ACTION_ENTRY);
            
                // Action delay
                actionBytes.push(padZero(10, theAction.delay));
            
                // Action data
                actionBytes.push(self.byteLocators.START_ACTION_DATA);
                actionBytes.push(theAction.command);
                actionBytes.push(self.byteLocators.END_ACTION_DATA);
                
                var actionInfo = actionBytes.join("");

                // Action length bytes
                macroBytes.push(padZero(5, actionInfo.length - 2));
                // Action bytes
                macroBytes.push(actionInfo);

                macroBytes.push(self.byteLocators.END_ACTION_ENTRY);
            }

            var macroInfo = macroBytes.join("");

            // Macro length bytes
            bytes.push(padZero(5, macroInfo.length - 2 - (theMacro.actions.length * 4)));
            // Macro bytes
            bytes.push(macroInfo);

            bytes.push(self.byteLocators.OVERALL_END_MACRO_ENTRY);
        }

        // Calculate the starting position of the rules, based on the flash memory starting position + length of the macro data already added above
        // Position is in hex, but ascii representation.
		var rulesStartAddress = bytes.join("").length;

        // Number of rules
        bytes.push(padZero(5, ruleCol.rules.length));

        // Loop through each rule
        var theRule;
        for (var i = 0; i < ruleCol.rules.length; i++) {
            theRule = ruleCol.rules[i];
            bytes.push(self.byteLocators.OVERALL_START_RULE);

			// Length of rule name
            bytes.push(padZero(5, theRule.name.length));
            // Rule name
            bytes.push(self.byteLocators.START_RULE_NAME);
            bytes.push(theRule.name);
            bytes.push(self.byteLocators.END_RULE_NAME);

            // Length of search string
            bytes.push(padZero(5, theRule.searchString.length));
            // Search string
            bytes.push(self.byteLocators.START_SEARCH_STRING);
            bytes.push(theRule.searchString);
            bytes.push(self.byteLocators.END_SEARCH_STRING);

            // Length of macro name
            bytes.push(padZero(5, theRule.macroName.length));
            // Macro name
            bytes.push(self.byteLocators.START_MAPPED_MACRO_NAME);
            bytes.push(theRule.macroName);
            bytes.push(self.byteLocators.END_MAPPED_MACRO_NAME);

            bytes.push(self.byteLocators.OVERALL_END_RULE);
        }

        bytes.push("ENDRLS");

        var byteString = bytes.join("");

        // Append filler bytes
        var numFillerBytes = self.byteLocators.BYTES_PER_PACKET - ((byteString.length + 8) % self.byteLocators.BYTES_PER_PACKET); // + 8 is allowing for rules starting address
        byteString += Array(numFillerBytes+1).join(self.byteLocators.FILLER_BYTE);

        // Add the rules starting position to the string after the memory address bytes
        //var rulesStartIncreaseBy = Math.ceil(rulesStartAddress / self.byteLocators.BYTES_PER_PACKET) * 8
        //console.log(rulesStartAddress + ", " + rulesStartIncreaseBy + ", " + parseInt(self.byteLocators.FLASH_MEMORY_POS, 16));
        // Calculate the rules start address taking into account number of flash memory bytes to be sliced into data
        rulesStartAddress = padZero(8, (rulesStartAddress + parseInt(self.byteLocators.FLASH_MEMORY_POS, 16) + 8).toString(16));

        byteString = rulesStartAddress + byteString
        
        // Split into an array of packets, each the correct number of bytes long
        var slices = byteString.match(/.{1,256}/g); // self.byteLocators.BYTES_PER_PACKET hardcoded here to 256 (change here as well as main definition if changed in future)
        
        for (var i = 0; i < slices.length; i++) {
            slices[i] = padZero(8, ((i * 256) + parseInt(self.byteLocators.FLASH_MEMORY_POS, 16)).toString(16)) + slices[i];
        }
        slices.push("END");
		
        // Return all the byte packets as an array
        return slices;
    };

    self.fromBytes = function (bytes, stripFlashAddressBytes) {
    	// Create the macro and rule objects from a big blob bytes

		// First clear any old objects
		RulesCollection.macros = [];
		RulesCollection.rules = [];

		if (stripFlashAddressBytes) {
	    	// First remove the Flash Start Address bytes
	    	var packets = bytes.match(/.{1,264}/g);
	    	for (var i = 0; i < packets.length; i++) {
	    		packets[i] = packets[i].substr(8);
	    	}
	    	bytes = packets.join("");
		}
		
		var baseRegex = /(.{8})(\d{5})(\x01.*\x02)(\d{5})(\x09.*\x0B)ENDRLS/;
		// Capture groups:
		// 1: Rules start address
		// 2: Number of macros
		// 3: Macro data
		// 4: Number of rules
		// 5: Rules data
		var macroRegex = /\x01\d{5}(\d{5})\x03(.*?)\x04(\d{5})(\x05.*?\x06(?!\x05))\x02/g;
		// Capture groups:
		// 1: Length of macro name
		// 2: Macro Name
		// 3: Number of actions
		// 4: Action data
		var actionRegex = /\x05\d{5}(\d{10})\x07(.*?)\x08\x06/g;
		// Capture groups:
		// 1: Delay time in milliseconds
		// 2: Command data
		var ruleRegex = /\x09\d{5}\x0C(.*?)\x0E\d{5}\x0F(.*?)\x10\d{5}\x11(.*?)\x12\x0B/g;
		// Capture groups:
		// 1: Search string
		// 2: Macro name
		
		var baseData = bytes.match(baseRegex);

		// Create the macros and their actions
		for (var i = 0; i < parseInt(baseData[2]); i++) {
			var macroData = macroRegex.exec(baseData[3]);
			if (macroData) {
				var newMacro = new RuleMacro(macroData[2]);
				for (var j = 0; j < parseInt(macroData[3]); j++) {
					var actionData = actionRegex.exec(macroData[4]);
					if (actionData) {
						newMacro.actions.push(new RuleMacroAction(parseInt(actionData[1]), actionData[2]));
					}
				}
				RulesCollection.macros.push(newMacro);
			}
		}
		
		// Create the rules
		for (var i = 0; i < parseInt(baseData[4]); i++) {
			var ruleData = ruleRegex.exec(baseData[5]);
			if (ruleData) {
				RulesCollection.rules.push(new Rule(ruleData[1], ruleData[2], ruleData[3]));
			}
		}

		//console.log(JSON.stringify(RulesCollection));
		
		return JSON.stringify(RulesCollection);
    };

    return self;

})();

var Rule = function (name, searchString, macroName) {
    var self = {
    	name: name || "",
        searchString: searchString || "",
        macroName: macroName || "",
    };

    return self;
};

var RuleMacro = function (name, actions) {
    var self = {
        name: name || "New Macro",
        actions: actions || [],
    };

    return self;
};

var RuleMacroAction = function (delay, command) {
    var self = {
        delay: delay || 0,
        command: command || "",
    };

    return self;
};

var RulesCollection = {
	macros: [],
	rules: []
};

/*
 * HELPER FUNCTIONS
 * ----------------
 */

function padZero(zeroes, stringToPad) {
    zeroes = zeroes || 2;
    stringToPad = stringToPad || "0";
    return (Array(zeroes+1).join("0")+stringToPad).slice(-zeroes);
}