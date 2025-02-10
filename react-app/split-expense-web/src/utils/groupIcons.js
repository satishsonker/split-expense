import {
    Home,
    Weekend,
    FlightTakeoff,
    Restaurant,
    LocalGroceryStore,
    Sports,
    Celebration,
    School,
    Work,
    FamilyRestroom,
    Groups,
    Apartment
} from '@mui/icons-material';

export const groupIcons = [
    { name: 'home', icon: Home, keywords: ['home', 'house', 'rent', 'apartment'] },
    { name: 'trip', icon: FlightTakeoff, keywords: ['trip', 'travel', 'vacation', 'flight', 'journey'] },
    { name: 'food', icon: Restaurant, keywords: ['food', 'dinner', 'lunch', 'restaurant', 'meal'] },
    { name: 'grocery', icon: LocalGroceryStore, keywords: ['grocery', 'shopping', 'market', 'store'] },
    { name: 'sports', icon: Sports, keywords: ['sports', 'game', 'fitness', 'gym'] },
    { name: 'party', icon: Celebration, keywords: ['party', 'celebration', 'event', 'birthday'] },
    { name: 'education', icon: School, keywords: ['education', 'school', 'college', 'study'] },
    { name: 'work', icon: Work, keywords: ['work', 'office', 'business', 'job'] },
    { name: 'family', icon: FamilyRestroom, keywords: ['family', 'parents', 'kids', 'relatives'] },
    { name: 'friends', icon: Groups, keywords: ['friends', 'buddies', 'mates'] },
    { name: 'apartment', icon: Apartment, keywords: ['apartment', 'flat', 'roommates'] },
    { name: 'weekend', icon: Weekend, keywords: ['weekend', 'leisure', 'relax'] }
];

export const getGroupIcon = (groupName) => {
    const normalizedName = groupName.toLowerCase();
    
    // Find the first matching icon based on keywords
    const matchedIcon = groupIcons.find(icon => 
        icon.keywords.some(keyword => normalizedName.includes(keyword))
    );

    // Return matched icon or default to Groups icon
    return matchedIcon?.icon || Groups;
}; 