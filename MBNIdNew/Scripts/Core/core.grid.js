// implement helpers for core js

//extend with helper namespace to provice privite variable and method
golbal.extendns(core, 'gridhelpers');
(function () {
    // deleare privite variable and method for core
}).apply(core.gridhelpers);

// extend with helper objects
golbal.extend(core, {
    gridhelpers: {
        getKendoGridInfo: function (grid) {
            var pageNumber = typeof grid == "undefined" ? 1 : grid.dataSource._page;
            var pageSize = typeof grid == "undefined" ? 10 : grid.dataSource._pageSize;
            var page = {
                GridId: 1,
                Page: pageNumber,
                PageSize: pageSize,
            };
            if (typeof grid != "undefined") {
                if (grid.dataSource._sort != undefined && grid.dataSource._sort.length > 0) {
                    page["SortColumn"] = grid.dataSource._sort[0].field;
                    page["SortDirection"] = grid.dataSource._sort[0].dir;
                }
            }
            return page;
        },
        getSearchTotalResult: function (elem) {
            if (typeof elem.dataSource != "undefined") {
                var section = elem.element.closest('section');
                if (section.find('span.search-result-number').length > 0)
                    section.find('span.search-result-number').text(elem.dataSource.total());
            }
        },
        refresh: function (grid) {
            if (grid && grid.dataSource) {
                grid.dataSource.page(1);
            }
        },
        refreshPage: function (grid, page) {
            if (grid && grid.dataSource) {
                page = typeof page == 'undefined' ? grid.dataSource._page : page;
                grid.dataSource.page(page);
            }
        },
        refreshPageSizeOrHide: function (grid) {
            if ($.isEmptyObject(grid.pager))
                return;

            var pagesize = $(grid.pager.element).find("select").data("kendoDropDownList");

            if ($.isEmptyObject(pagesize))
                return;

            // restore
            var pageSizes = [{ text: "10", value: 10 }, { text: "20", value: 20 }, { text: "30", value: 30 }, { text: "40", value: 40 }, { text: "50", value: 50 }, { text: "100", value: 100 }];
            pagesize.setDataSource(new kendo.data.DataSource({ data: pageSizes }));

            var totalRow = grid.dataSource.total();
            var dataSource = pagesize.dataSource;
            var length = pagesize.dataSource.data().length;

            var item, i, remove = false;
            for (i = 0; i < length; i++) {
                item = pagesize.dataSource.at(i);
                if (typeof (item) == "undefined")
                    continue;

                if (remove) {
                    pagesize.dataSource.remove(item);
                    i--;
                }

                if (parseInt(item.value) >= totalRow && !remove) {
                    remove = true;
                }
            }

            if (grid.dataSource.totalPages() <= 1) {
                grid.pager.element.hide();
            } else {
                grid.pager.element.show();
            }
        },
        //private method, please not replace or set value for this function
        raiseEvent: function (e, eventName) {
            var _event = e.sender.element.context.attributes[eventName];
            if (_event != null)
                eval(_event.value + '(e)');

        },
        databinding: function (e) {
            //set tooltip for header
            var grid = e.sender;

            // hide pager if grid using scroll mode
            core.gridhelpers.scroll.hidePager(grid);

            $(grid.thead.find('th')).each(function () {
                if (!$(this).prop('title')) {
                    $(this).prop('title', $(this).data('title'));
                }
            });
        },
        //private method, please not replace or set value for this function
        databound: function (e) {

            core.gridhelpers.getSearchTotalResult(e.sender);
            core.gridhelpers.refreshPageSizeOrHide(e.sender);

            //call customer method
            var grid = e.sender, wrapperDataNotFound;
            core.gridhelpers.raiseEvent(e, 'as-event-databound');

            //show/hide message datanotfound
            wrapperDataNotFound = $("div[as-datanotfound-for-grid='" + grid.element.context.attributes['id'].value + "']");
            if (grid.dataSource._data.length > 0) {
                core.helper.show(grid.element);
                grid.element.removeClass('visible-hide');
                core.helper.hide(wrapperDataNotFound);
                grid.element.find('td').each(function () {
                    if (this.children.length == 0 && $(this).text().trim() == "")
                        $(this).text("—");
                });
            } else {
                core.helper.hide(grid.element);
                core.helper.show(wrapperDataNotFound);
            }

            //cset default behavior select or not select for checkbox select all
            var chkSelectAll = $(grid.element).find('input[as-gird-selectall]');
            if (chkSelectAll.length > 0)
                core.gridhelpers.selectRow.setBehaviorSelectAll(chkSelectAll[0], grid);
        },

        selectRow: {
            selectHandler: function (e, callback) {
                var isValid = false, grid;
                grid = $(e.target).closest('div.k-grid').data('kendoGrid');

                if (typeof callback != 'undefined')
                    isValid = eval(callback + '(e)');

                //set behavior when callback return true
                if (isValid)
                    core.gridhelpers.selectRow.setBehaviorSelectAll(e.target, grid);
            },
            selectAllHandler: function (e, callback) {
                var grid = $(e.target).closest('div.k-grid').data('kendoGrid');
                $(grid.element).find('input[select-input]').prop('checked', $(e.target).prop('checked'));

                if (typeof callback != 'undefined')
                    eval(callback + '(e)');
            },

            setBehaviorSelectAll: function (sender, grid) {
                if ($(sender).attr('type') == 'checkbox') {
                    if ($(grid.element).find('input[select-all-input]').length > 0) {
                        var limitSelected = 0, totalSelected = 0;
                        var allInPage = sender.attributes['as-select-all-in-page'].value;
                        if (allInPage == 'True' || allInPage == true) {
                            limitSelected = grid.dataSource.pageSize() > grid.dataSource.total() ? grid.dataSource.total() : grid.dataSource.pageSize();
                            $(grid.element).find('input[select-input]').each(function () {
                                if ($(this).prop('checked'))
                                    totalSelected = totalSelected + 1;
                            })
                        } else {
                            limitSelected = grid.dataSource._total;
                            totalSelected = eval(sender.attributes['as-event-get-total-selected'].value + ('(sender,grid)'));
                        }
                        $(grid.element).find('input[select-all-input]').prop('checked', totalSelected == limitSelected);
                    }
                }
            }
        },
        //define and maintance scroll of grid
        scroll: {
            init: function (kgrid) {
                kgrid = $(document.getElementById(kgrid)).data('kendoGrid');
                var kcontent, kscroll
                if (typeof kgrid === 'undefined') {
                    alert('can not access grid because grid not created');
                    return false;
                }

                //init start]
                if (typeof kgrid.element.data('asScroll') == 'undefined') {
                    $.extend(core.gridhelpers.scroll.data.dataSource, kgrid.dataSource);
                    kgrid.element.data('asScroll', core.gridhelpers.scroll.data);
                }

                kcontent = kgrid.element.find('.k-grid-content');
                kcontent.addClass('as-scroll');
                var scrollData = kgrid.element.data('asScroll');
                kcontent.on('scroll', function () {
                    var that = $(this);
                    if (that.scrollTop() + that.innerHeight() >= that[0].scrollHeight) {
                        if (scrollData.dataSource.page() * scrollData.dataSource.pageSize() < scrollData.dataSource.total()) {
                            scrollData.dataSource.next();
                            var len = scrollData.dataSource.data().length;
                            for (var i = 0; i < len; i++) {
                                kgrid.dataSource.add(scrollData.dataSource.data()[i]);
                            }
                            this.focus = false;
                            //update paging info
                            setTimeout(function () {
                                $(kgrid.element.parent()).find('.k-pager-info').text('1-' + scrollData.dataSource.page() * scrollData.dataSource.pageSize() + ' of ' + scrollData.dataSource.total());
                            }, 500);

                        }
                    }
                })

            },
            data: {
                dataSource: new kendo.data.DataSource(),
            },
            heplers: {
                getRoot: function (grid) {
                    if (typeof grid == 'undefined')
                        return null;

                    var root = $(grid.element).closest('div.as-grid-scroll');
                    if (typeof root == 'undefined')
                        return null;
                    return root;
                }
            },
            hidePager: function (grid) {
                var root = core.gridhelpers.scroll.heplers.getRoot(grid);
                if (root != null)
                    core.helper.hide(root.find('.k-pager-wrap'));
            },
        }
    }
});



golbal.extendns(core, 'vcardhelper');
// extend with helper objects
golbal.extend(core, {
    viewcardhelpers: {
        cardinit: function (viewid) {

        },
        consts: {
            ROW: 'div[card-row]',
            CHECK: 'input[select-input]',
            ITEM_CHECKED: 'rowChecked',
            SelectButton: {
                UN: 'fa-square-o',
                MULTI: 'fa-minus-square-o',
                ALL: 'fa-check-square'
            }
        },
        //private method, please not replace or set value for this function
        raiseEvent: function (e, eventName) {
            var _event = e.sender.element.context.attributes[eventName];
            if (_event != null)
                eval(_event.value + '(e)');

        },
        //private method, please not replace or set value for this function
        databound: function (e) {
            core.gridhelpers.getSearchTotalResult(e.sender);
            core.gridhelpers.refreshPageSizeOrHide(e.sender);

            kendo.ui.progress(e.sender.element, true);
            //call customer method
            var view = e.sender, wrapperDataNotFound;
            core.viewcardhelpers.raiseEvent(e, 'as-event-databound');

            //show/hide meesage datanotfound
            wrapperDataNotFound = $("div[as-datanotfound-for-card='" + view.element.context.attributes['id'].value + "']");
            if (view.dataSource._data.length > 0) {
                core.helper.show(view.element.parent());
                view.element.removeClass('visible-hide');
                core.helper.hide(wrapperDataNotFound);
            } else {
                core.helper.hide(view.element.parent());
                core.helper.show(wrapperDataNotFound);
            }
            kendo.ui.progress($("body"), false);
        },
        refresh: function (view) {
            var viewcard;
            if (typeof viewid == 'object')
                viewcard = view;
            else
                viewcard = $(document.getElementById(view)).data('kendoListView');
            if (viewcard && viewcard.dataSource) {
                viewcard.dataSource.page(1);
            }
        }
        ,
        row: {
            //select all handler
            selectAll: function (e, callback) {
                var selected = false;
                var sender = $(e.target);
                if (sender.tagName != 'BUTTON')
                    sender = sender.closest('button');

                var icon = $(sender).find('span.fa');
                if (icon.hasClass(core.viewcardhelpers.consts.SelectButton.ALL)) {
                    icon.removeClass(core.viewcardhelpers.consts.SelectButton.ALL).addClass(core.viewcardhelpers.consts.SelectButton.UN);
                    selected = false;
                }
                else {
                    icon.removeClass(core.viewcardhelpers.consts.SelectButton.UN).removeClass(core.viewcardhelpers.consts.SelectButton.MULTI).addClass(core.viewcardhelpers.consts.SelectButton.ALL);
                    selected = true;
                }

                $(document.getElementById(sender.attr('for-card'))).find(core.viewcardhelpers.consts.CHECK).prop('checked', selected);

                eval(callback + '(e,selected)');
            },
            selectHandler: function (e, callback) {

                var card = $(e.target).closest('div.k-listview');
                var dataSource = card.data('kendoListView').dataSource;
                var inputSelectAll = $('button[for-card="' + card.attr('id') + '"]');
                if (typeof inputSelectAll != "undefined") {
                    var selectAllCss = '';
                    var selectAllInPage = inputSelectAll.attr('select-all-inpage');
                    var totalSelected = 0;

                    if (selectAllInPage == 'true') {
                        card.find(core.viewcardhelpers.consts.CHECK).each(function () {
                            totalSelected += $(this).prop('checked') ? 1 : 0;
                        });
                        if (totalSelected == dataSource.pageSize()) {
                            selectAllCss = core.viewcardhelpers.consts.SelectButton.ALL;
                        } else {
                            selectAllCss = totalSelected > 0 ? core.viewcardhelpers.consts.SelectButton.MULTI : core.viewcardhelpers.consts.SelectButton.UN;
                        }
                    } else {
                        totalSelected = eval(inputSelectAll.attr('getTotalSelectedHandler') + '(e)');

                        if (totalSelected == dataSource.total())
                            selectAllCss = core.viewcardhelpers.consts.SelectButton.ALL;
                        else
                            selectAllCss = totalSelected > 0 ? core.viewcardhelpers.consts.SelectButton.MULTI : core.viewcardhelpers.consts.SelectButton.UN;
                    }

                    //reset class for select all button;
                    inputSelectAll.find('span.fa').removeClass(core.viewcardhelpers.consts.SelectButton.UN).removeClass(core.viewcardhelpers.consts.SelectButton.MULTI).removeClass(core.viewcardhelpers.consts.SelectButton.ALL).addClass(selectAllCss);
                }
                //raise event calback
                eval(callback + '(e)');
            },
            click: function (e, callback) {
                var $wrapper, ctarget;
                if ($(e.target).is(core.viewcardhelpers.consts.ROW)) {
                    $wrapper = $(e.target);
                } else {
                    if ($(e.target).is('a,input')) {
                        e.stopPropagation();
                        return false;
                    }
                    $wrapper = $(e.target).closest(core.viewcardhelpers.consts.ROW);
                }

                if (callback != null || callback != '') {
                    var _data = core.viewcardhelpers.row.getData($wrapper);
                    eval(callback + '(e,_data)');
                }
            },
            getData: function (_wrapper) {
                var item, $wrapper, view, datas;
                if (typeof _wrapper == 'object') {
                    _wrapper = $(_wrapper);
                    if ($(_wrapper).is(core.viewcardhelpers.consts.ROW))
                        $wrapper = _wrapper;
                    else {
                        //check if _wrapper is event then get target
                        if (_wrapper[0].target != null)
                            _wrapper = $(_wrapper[0].target);

                        $wrapper = _wrapper.closest(core.viewcardhelpers.consts.ROW);
                    }
                } else {
                    $wrapper = $(document.getElementById(_wrapper));
                }
                item = $.map($wrapper.closest('.k-listview').data('kendoListView').dataSource._data, function (o) {
                    return o.uid == $wrapper.attr('data-uid') ? o : null;
                })[0]

                if (_wrapper != null && $(_wrapper).is(core.viewcardhelpers.consts.CHECK))
                    item[core.viewcardhelpers.consts.ITEM_CHECKED] = $(_wrapper).prop('checked');

                return item;
            }

        },
        sort: {
            operatorHandler: function (e) {
                var operator = e.target.getAttribute('data-operator');
                operator = operator.toLowerCase() == 'desc' ? 'ASC' : 'DESC';
                e.target.setAttribute('data-operator', operator);
                e.target.innerText = operator;
                core.viewcardhelpers.sort.sorting(e);
            },
            fieldChangeHandler: function (e) {
                core.viewcardhelpers.sort.sorting(e);
            },
            sorting: function (e) {
                var sortFor = e.target.closest('div[as-sort]');
                if (typeof sortFor != "undefined") {
                    var sDir = sortFor.querySelector('button[data-operator]').getAttribute('data-operator');
                    var sField = sortFor.querySelector('input[id="sortForViewCard"]').value;

                    var dataSource = $(document.getElementById(sortFor.getAttribute('as-sort'))).data('kendoListView').dataSource;
                    var _sort = null;
                    if (sField.trim() != '') {
                        _sort = { compare: null, dir: sDir.toLowerCase(), field: sField };
                    }
                    dataSource._sort = _sort;
                    dataSource.page(dataSource.page());
                }
            }
        }
    }
});