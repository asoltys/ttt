define('data-manager', ['jquery-private', 'moment', 'helper'], function($, moment, h) {
    'use strict';
    console.info('running data-manager.js');
    // constants & config variables
    var CULTURE;
    var CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F';
    var NAME_CULTURE = 'Name' + CULTURE_APPEND;
    var TEXT_CULTURE = 'Text' + CULTURE_APPEND;
    var DESC_CULTURE = 'Description' + CULTURE_APPEND;
    var HOVER_CULTURE = 'Hover' + CULTURE_APPEND;
    var API_DATE_FORMAT = 'MM/DD/YYYY';
    var TIMELINES_URL = '/data/all';
    var REGIONS_URL = '/data/regions';
    var BRANCHES_URL = '/data/branches';
    var TIMELINE_BUFFER = 3;
    var TIMELINE_BUFFER_UNIT = 'month';
    var GET = 'get';
    var POST = 'post';
    var TIMELINES, REGIONS, BRANCHES, TIMESPAN;
    
    // local variables used for sorting & filtering & retrieving values
    var timelines, regions, branches;
    var timeline = 'all', region = 'all', branch = 'all';
    var momentObject1 = moment();
    var momentObject2 = moment();
    
    var _prepareData = function() {
        _prepareRegions();
        _prepareBranches();
        _findTimespan();
        _deepCopyData();
    }

    var _prepareRegions = function() {
        REGIONS = _sortByName(REGIONS);
        var nca;
        REGIONS = REGIONS.filter(function (object) {
            if (object.NameShort == "nca")
                nca = object;
            return object.NameShort != "nca"
        });
        REGIONS.unshift(nca);
    }

    var _prepareBranches = function() {
        BRANCHES = _sortByName(BRANCHES);
    }

    var _findTimespan = function() {
        var earliestDate, latestDate, startDates = [], endDates = [];
        TIMELINES.forEach(function(timeline, index, array) {
            timeline.Data.forEach(function(initiative, index, array) {
                startDates.push(moment(initiative.StartDate, API_DATE_FORMAT));
                endDates.push(moment(initiative.EndDate, API_DATE_FORMAT));
            });
            earliestDate = startDates.reduce(function(a, b) { return a < b ? a : b; });
            latestDate = endDates.reduce(function(a, b) { return a > b ? a : b; });
        });
        earliestDate.subtract(TIMELINE_BUFFER, TIMELINE_BUFFER_UNIT);
        latestDate.add(TIMELINE_BUFFER, TIMELINE_BUFFER_UNIT);
        var duration = moment.duration(latestDate.diff(earliestDate));
        TIMESPAN = { Start: earliestDate, End: latestDate,
            Duration: {
                Days: Math.ceil(duration.asDays()),
                Months: Math.ceil(duration.asMonths()),
                Years: Math.ceil(duration.asYears())
            }
        };
    }

    var _deepCopyData = function() {
        timelines = h.jsonCopy(TIMELINES);
        regions = h.jsonCopy(REGIONS);
        branches = h.jsonCopy(BRANCHES);
    }

    var _sortByName = function(data) {
        return data.sort(function(a, b) {
            return h.sortComparator(a['Name' + CULTURE_APPEND], b['Name' + CULTURE_APPEND]);
        });
    }

    var _sortByImpact = function(data) {
        return data.sort(function(a, b) {
            if (b.Weight == 4) return false;
            return h.sortComparator(b.Weight, a.Weight);
        })
    }

    var _filterTimeline = function(value) {
        timelines = h.jsonCopy(TIMELINES);
        if (timeline == 'all') {
            timelines.forEach(function(timeline) {
                _sortByName(timeline.Data);
            });
            return;
        }
        timelines.forEach(function(timelineParam) {
            timelineParam.Skip = timelineParam[NAME_CULTURE] == timeline ? false : true;
        });
    }
    
    var _config = function(config) {
        if (h.keyExists('CULTURE', config)) {
            CULTURE = config.CULTURE;
            CULTURE_APPEND = CULTURE == 'en-ca' ? 'E' : 'F';
        }
    }

    var _filterAll = function() {
        _filterTimeline(timeline);
        if (region == 'all' || branch == 'all') return;
        var weight;
        timelines.forEach(function(timeline) {
            timeline.Data.forEach(function(initiative) {
                // save weight to data
                weight = h.getValueByKey(_getControl(), initiative.Impacts);
                initiative.Weight = weight == false ? 0 : weight;
                // save event skip data
                initiative.Events.forEach(function(event) {
                    event.Skip = !h.keyExists(_getControl(), event.Control);
                });
            });
            // sort by weight then name
            timeline.Data.sort(function(a, b) {
                return (h.sortComparator(b.Weight, a.Weight) ||
                    h.sortComparator(a[NAME_CULTURE], b[NAME_CULTURE]));
            });
        });
    }
    
    var _loadData = function(callback) {
        var xhr1 = h.ajax(POST, TIMELINES_URL, function(data) {
            TIMELINES = JSON.parse(data);
        }, {culture: CULTURE});
        var xhr2 = h.ajax(POST, REGIONS_URL, function(data) {
            REGIONS = JSON.parse(data);
        });
        var xhr3 = h.ajax(POST, BRANCHES_URL, function(data) {
            BRANCHES = JSON.parse(data);
        });
        $.when(xhr1, xhr2, xhr3).done(function() {
            _prepareData();
            callback();
        });
    }
    
    // there are only 2 moment objects in data manager
    // object index needs to be defined to return the proper object
    var _momentize = function(date, objectIndex) {
        date = date == '' ? moment().format('MM/DD/YY').split('/') : date.split('/');
        if (objectIndex == 1) {
            momentObject1.set('month', (date[0] - 1));
            momentObject1.set('date', date[1]);
            momentObject1.set('year', date[2]);
            return momentObject1;
        } else {
            momentObject2.set('month', (date[0] - 1));
            momentObject2.set('date', date[1]);
            momentObject2.set('year', date[2]);
            return momentObject2;
        }
    }

    var _momentDiff = function() {
        return momentObject2.diff(momentObject1);
    }

    var _getTimelines = function() { return timelines; }
    var _getRegions   = function() { return regions; }
    var _getBranches  = function() { return branches; }
    var _getTimespan  = function() { return TIMESPAN; }
    var _getControl = function() { return region + ',' + branch };
    var _getControlObject = function() { return { region: region, branch: branch } };
    var _setBranch = function(key) { branch = key; }
    var _setRegion = function(key) { region = key; }
    var _setTimeline = function(key) { timeline = key; }

    return {
        config: _config,
        filter: _filterAll,
        load: _loadData,
        momentize: _momentize,
        momentDiff: _momentDiff,
        
        // getters and setters open for public
        branches: _getBranches,
        regions: _getRegions,
        timelines: _getTimelines,
        timespan: _getTimespan,
        getControl: _getControl,
        getControlObject: _getControlObject,
        setBranch: _setBranch,
        setRegion: _setRegion,
        setTimeline: _setTimeline
    }
}, function(err) {
    console.log(err.requireModules);
});
