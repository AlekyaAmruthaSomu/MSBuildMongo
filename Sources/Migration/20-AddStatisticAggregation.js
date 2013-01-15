db.system.js.save(
  {
    _id : "CockpitStatisticAggregation",
    value : function (startYear, startMonth, startDay, startHours, startMinutes, endYear, endMonth, endDay, endHours, endMinutes) {
              var getDate = function(year, month, day, hours, minutes)
              {
                var date = new Date(0);
                date.setUTCFullYear(year);	
                date.setUTCMonth(month - 1);	
                date.setUTCDate(day);
                date.setUTCHours(hours);
                date.setUTCMinutes(minutes);
                return date;	
              }
              
              var aggregate = function(startStroke, endStroke)
              {	
                return db.Sessions.aggregate(
                  {'$match':{'StartDate':{'$lt': endStroke }, 'EndDate':{'$gt': startStroke}}},
                  {'$project':{ '_id' : 0, 'DateTime' : { '$toUpper' : startStroke }}},
                  {'$group' : { '_id' : {'DateTime':'$DateTime'}, 'Count' : { '$sum' : 1}}}			
                ).result;		
              }
              
              var lastRun = db.task.findOne();
              print('lastRun: ' + lastRun.lastrun);
              
              var delta = 300000;
              var startDate = getDate(startYear, startMonth, startDay, startHours, startMinutes);		
              var endDate = getDate(endYear, endMonth, endDay, endHours, endMinutes);	
              var duration = endDate.getTime() - startDate.getTime();
              
              var maxCount = duration / delta;
              
              
              var r = [];
              print('count: ' + maxCount);
              for(var i = 0; i < maxCount; i++)
              {
                var startStroke = new Date(startDate.getTime() + delta * i);
                var endStroke = new Date(startStroke.getTime() + delta);
                r[i] = aggregate(startStroke, endStroke);
              }
              return r;
            }
  }
);

