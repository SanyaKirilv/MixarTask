const ships = {
    A: { health: 100, maxHealth: 100, shield: 80, maxShield: 80, shieldRecovery: 1, weapons: [], modules: [], weaponSlots: 2, moduleSlots: 2 },
    B: { health: 60, maxHealth: 60, shield: 120, maxShield: 120, shieldRecovery: 1, weapons: [], modules: [], weaponSlots: 2, moduleSlots: 3 }
};

const weapons = {
    A: { name: "Пушка A", cooldown: 3, damage: 5, currentCooldown: 0 },
    B: { name: "Пушка B", cooldown: 2, damage: 4, currentCooldown: 0 },
    C: { name: "Пушка C", cooldown: 5, damage: 20, currentCooldown: 0 }
};

const modules = {
    A: { name: "Модуль A", type: "shield", value: 50 },
    B: { name: "Модуль B", type: "health", value: 50 },
    C: { name: "Модуль C", type: "cooldown", value: 0.2 },
    D: { name: "Модуль D", type: "shieldRecovery", value: 0.2 }
};

let battleInterval;

function populateSelectOptions() {
    const moduleSelectA = document.getElementById('moduleSelectA');
    const moduleSelectB = document.getElementById('moduleSelectB');
    const weaponSelectA = document.getElementById('weaponSelectA');
    const weaponSelectB = document.getElementById('weaponSelectB');

    for (const key in modules) {
        const optionA = document.createElement('option');
        const optionB = document.createElement('option');
        optionA.value = key;
        optionA.textContent = modules[key].name;
        optionB.value = key;
        optionB.textContent = modules[key].name;
        moduleSelectA.appendChild(optionA);
        moduleSelectB.appendChild(optionB);
    }

    for (const key in weapons) {
        const optionA = document.createElement('option');
        const optionB = document.createElement('option');
        optionA.value = key;
        optionA.textContent = weapons[key].name;
        optionB.value = key;
        optionB.textContent = weapons[key].name;
        weaponSelectA.appendChild(optionA);
        weaponSelectB.appendChild(optionB);
    }
}

function installModule(shipId) {
    const selectId = shipId === 'A' ? 'moduleSelectA' : 'moduleSelectB';
    const selectedModuleKey = document.getElementById(selectId).value;

    if (!selectedModuleKey) {
        alert('Пожалуйста, выберите модуль.');
        return;
    }

    if (ships[shipId].modules.length >= ships[shipId].moduleSlots) {
        alert(`На корабле ${shipId} нет свободных слотов для модулей!`);
        return;
    }

    const module = modules[selectedModuleKey];
    ships[shipId].modules.push(module);

    // Применяем эффекты модулей
    if (module.type === "shield") {
        ships[shipId].maxShield += module.value;
        ships[shipId].shield += module.value;
    } else if (module.type === "health") {
        ships[shipId].maxHealth += module.value;
        ships[shipId].health += module.value;
    } else if (module.type === "cooldown") {
        ships[shipId].weapons.forEach(weapon => {
            weapon.cooldown *= (1 - module.value);
        });
    } else if (module.type === "shieldRecovery") {
        ships[shipId].shieldRecovery *= (1 + module.value);
    }

    console.log(`Модуль ${module.name} успешно установлен на корабль ${shipId}`);
    updateInstalledModulesUI(shipId);
}

function installWeapon(shipId) {
    const selectId = shipId === 'A' ? 'weaponSelectA' : 'weaponSelectB';
    const selectedWeaponKey = document.getElementById(selectId).value;

    if (!selectedWeaponKey) {
        alert('Пожалуйста, выберите оружие.');
        return;
    }

    if (ships[shipId].weapons.length >= ships[shipId].weaponSlots) {
        alert(`На корабле ${shipId} нет свободных слотов для оружия!`);
        return;
    }

    const weapon = { ...weapons[selectedWeaponKey] };
    ships[shipId].weapons.push(weapon);
    console.log(`Оружие ${weapon.name} успешно установлено на корабль ${shipId}`);
    updateInstalledWeaponsUI(shipId);
}

function startBattle() {
    battleInterval = setInterval(() => {
        for (const shipId in ships) {
            const ship = ships[shipId];
            const opponentId = shipId === 'A' ? 'B' : 'A';
            const opponent = ships[opponentId];

            if (ship.shield < ship.maxShield) {
                ship.shield = Math.min(ship.shield + ship.shieldRecovery, ship.maxShield);
            }

            ship.weapons.forEach(weapon => {
                if (weapon.currentCooldown > 0) {
                    weapon.currentCooldown--;
                } else {
                    if (opponent.shield > 0) {
                        opponent.shield -= weapon.damage;
                        if (opponent.shield < 0) {
                            opponent.health += opponent.shield;
                            opponent.shield = 0;
                        }
                    } else {
                        opponent.health -= weapon.damage;
                    }

                    logEvent(`${shipId} наносит ${weapon.damage} урона кораблю ${opponentId}`);
                    weapon.currentCooldown = weapon.cooldown;
                }
            });

            if (opponent.health <= 0) {
                logEvent(`Корабль ${opponentId} потерпел поражение!`);
                clearInterval(battleInterval);
                return;
            }
        }

        updateUI();
    }, 1000);
}

function updateUI() {
    document.getElementById('shipAHealth').textContent = ships.A.health;
    document.getElementById('shipAShield').textContent = ships.A.shield;
    document.getElementById('shipBHealth').textContent = ships.B.health;
    document.getElementById('shipBShield').textContent = ships.B.shield;
}

function updateInstalledModulesUI(shipId) {
    const listId = shipId === 'A' ? 'installedModulesA' : 'installedModulesB';
    const list = document.getElementById(listId);
    list.innerHTML = '';
    ships[shipId].modules.forEach(module => {
        const listItem = document.createElement('li');
        listItem.textContent = module.name;
        list.appendChild(listItem);
    });
}

function updateInstalledWeaponsUI(shipId) {
    const listId = shipId === 'A' ? 'installedWeaponsA' : 'installedWeaponsB';
    const list = document.getElementById(listId);
    list.innerHTML = '';
    ships[shipId].weapons.forEach(weapon => {
        const listItem = document.createElement('li');
        listItem.textContent = weapon.name;
        list.appendChild(listItem);
    });
}

function logEvent(message) {
    const log = document.getElementById('log');
    const listItem = document.createElement('li');
    listItem.textContent = message;
    log.appendChild(listItem);
}

populateSelectOptions();
updateInstalledModulesUI('A');
updateInstalledModulesUI('B');
updateInstalledWeaponsUI('A');
updateInstalledWeaponsUI('B');
updateUI();
