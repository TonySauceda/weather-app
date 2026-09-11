const favoritesStorageKey = 'weather-app.favorite-cities';

export function loadFavorites() {
    try {
        const storedFavorites = localStorage.getItem(favoritesStorageKey);
        return storedFavorites ? JSON.parse(storedFavorites) : [];
    } catch {
        return [];
    }
}

export function saveFavorites(favorites) {
    localStorage.setItem(favoritesStorageKey, JSON.stringify(favorites));
}
