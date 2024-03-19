var PagedTable = {
	SortDirection: {
		Ascending: 0,
		Descending: 1
	},
	GetPageSize: function (tableContainer) {
		return parseInt(tableContainer.find("input.page-size").first().val() || "0");
	},
	GetSearchTerm: function (tableContainer) {
		return $.trim(tableContainer.find(".search-term").val());
	},
	GetRowCount: function (tableContainer) {
		var searchTerm = PagedTable.GetSearchTerm(tableContainer);
		var pagedTable = tableContainer.find(".paged-table");
		var allRows = pagedTable.find("tr");
		var visibleRowsCount = pagedTable.find("tr:visible").length - 1; ;
		var allRowsCount = searchTerm.length > 0 ? pagedTable.find(".search-result").length : allRows.length - 1;
		return {
			VisibleRowsCount: visibleRowsCount,
			AllRowsCount: allRowsCount
		};
	},
	HandleRecordsCount: function () {
		function ConstructMessage(tableContainer, allRowsCount, visibleRowsCount, firstRowIndex) {
			var singularCaption = tableContainer.find(".singular-caption").val() || "רשומה";
			var pluralCaption = tableContainer.find(".plural-caption").val() || "רשומות";
			var captionGender = parseInt(tableContainer.find(".caption-gender").val() || "2");
			if (visibleRowsCount == 0)
				return "אין " + pluralCaption;
			if (visibleRowsCount == 1)
				return singularCaption + " " + ((captionGender == 1) ? "אחד" : "אחת");
			if (allRowsCount <= visibleRowsCount)
				return visibleRowsCount + " " + pluralCaption;
			var message = pluralCaption + " " + firstRowIndex + "-" + (firstRowIndex + visibleRowsCount - 1);
			message += " מתוך " + allRowsCount;
			return message;
		}

		$(".records-count").each(function () {
			var recordCountSpan = $(this);
			var tableContainer = recordCountSpan.parents(".paged-table-container").first();
			var pagedTable = tableContainer.find(".paged-table").first();
			var searchTerm = PagedTable.GetSearchTerm(tableContainer);
			var rowCounts = PagedTable.GetRowCount(tableContainer);
			var firstRowIndex = pagedTable.find("tr").filter(function () { return $(this).is(":visible"); }).eq(1).index();
			if (searchTerm.length > 0)
				firstRowIndex -= (pagedTable.find("tr").filter(function () { return $(this).hasClass("search-result"); }).eq(0).index() - 1);
			var message = ConstructMessage(tableContainer, rowCounts.AllRowsCount, rowCounts.VisibleRowsCount, firstRowIndex);
			recordCountSpan.html(message);
			var additionalButtons = tableContainer.find(".additional-buttons-placeholder").find("button");
			var disabled = (rowCounts.VisibleRowsCount == 0) ? "disabled" : "";
			additionalButtons.prop("disabled", disabled);
		});
	},
	HandleColumnSorting: function () {
		function GetSortOrder(headerCell) {
			var descendingOrderLabel = headerCell.find(".descending-order");
			if (descendingOrderLabel.is(":visible"))
				return PagedTable.SortDirection.Ascending;
			return PagedTable.SortDirection.Descending;
		}

		function ApplySortIcons(pagedTable, headerCell, sortOrder) {
			pagedTable.find(".descending-order").hide();
			pagedTable.find(".ascending-order").hide();
			var className = (sortOrder == PagedTable.SortDirection.Ascending) ? "ascending-order" : "descending-order";
			headerCell.find("." + className).show();
		}

		function CompareRows(r1, r2, cellIndex, sortOrder) {
			var c1 = $(r1).find("td").eq(cellIndex);
			var c2 = $(r2).find("td").eq(cellIndex);
			var v1 = c1.data("value") || $.trim(c1.text());
			var v2 = c2.data("value") || $.trim(c2.text());
			if (v1 == v2)
				return 0;
			var num = (sortOrder == PagedTable.SortDirection.Ascending) ? 1 : -1;
			if (v1 > v2)
				return num;
			return num * -1;
		}

		$(".paged-table").each(function () {
			var pagedTable = $(this);
			var tableContainer = pagedTable.parents(".paged-table-container").first();
			pagedTable.find("th").bind("click", function () {
				var headerCell = $(this);
				var cellIndex = headerCell.index();
				var sortOrder = GetSortOrder(headerCell);
				var rows = pagedTable.find("tr:gt(0)");
				var pageSize = PagedTable.GetPageSize(tableContainer);
				rows.sort(function (r1, r2) {
					return CompareRows(r1, r2, cellIndex, sortOrder);
				});
				rows.detach();
				pagedTable.append(rows);
				ApplySortIcons(pagedTable, headerCell, sortOrder);
				if (PagedTable.GetSearchTerm(tableContainer).length > 0) {
					PagedTable.ApplySearchTerm(tableContainer);
				} else {
					PagedTable.Pager.SetCurrentPage(tableContainer.find(".pages-placeholder").first(), 1);
				}
			});
		});
	},
	HandleRowLinks: function () {
		$(".paged-table tr").each(function () {
			var row = $(this);
			var targetUrl = row.data("target-url") || "";
			if (targetUrl.length > 0) {
				row.css("cursor", "pointer");
				row.bind("click", function () {
					window.location.href = targetUrl;
				});
			}
		});
	},
	ApplySearchTerm: function (tableContainer) {
		var searchTerm = PagedTable.GetSearchTerm(tableContainer);
		var pagedTable = tableContainer.find(".paged-table");
		var pageSize = PagedTable.GetPageSize(tableContainer);
		var allRows = pagedTable.find("tr:gt(0)");
		allRows.removeClass("search-result");
		allRows.hide();
		if (searchTerm.length > 0) {
			var matchingRows = allRows.filter(function () {
				var contents = $.trim($(this).text());
				return contents.toLowerCase().indexOf(searchTerm.toLowerCase()) >= 0;
			});
			matchingRows.addClass("search-result");
		}
		PagedTable.Pager.SetCurrentPage(tableContainer.find(".pages-placeholder").first(), 1);
	},
	HandleSearch: function () {
		$(".paged-table-container").each(function () {
			var tableContainer = $(this);
			var searchTextbox = tableContainer.find(".search-term");
			if (searchTextbox.length == 1) {
				var searchTimer = 0;
				searchTextbox.bind("keyup mouseup paste", function () {
					var currentValue = $.trim(searchTextbox.val());
					var previousValue = searchTextbox.data("previous-value") || "";
					if (currentValue != previousValue) {
						window.clearTimeout(searchTimer);
						searchTimer = window.setTimeout(function () {
							PagedTable.ApplySearchTerm(tableContainer);
						}, 200);
						searchTextbox.data("previous-value", currentValue)
					}
				});
			}
		});
	},
	HandleAdditionalButtons: function () {
		$(".paged-table-container").each(function () {
			var tableContainer = $(this);
			var buttonsPlaceholder = tableContainer.find(".additional-buttons-placeholder");
			var additionalButtonsArray = tableContainer.find(".additional-buttons").val().split(",");
			var addedButtonsCount = 0;
			for (var i = 0; i < additionalButtonsArray.length; i++) {
				var additionalButtonId = additionalButtonsArray[i];
				if (additionalButtonId.length > 0) {
					var additionalButton = $("#" + additionalButtonId);
					if (additionalButton.length == 1) {
						buttonsPlaceholder.append(additionalButton);
						addedButtonsCount++;
					}
				}
			}
			if (addedButtonsCount > 0)
				buttonsPlaceholder.show();
		});
	},
	Pager: {
		SetCurrentPage: function (pagesPlaceholder, curPage) {
			function IsPageIncluded(page, pageCount) {
				if (page == 1 || page == pageCount)
					return true;
				var medianPage = curPage;
				if (medianPage < 3)
					medianPage = 3;
				if (medianPage > (pageCount - 2))
					medianPage = (pageCount - 2);
				return (page == medianPage) || (page == (medianPage - 1)) || (page == (medianPage + 1));
			}

			function BuildPages(pageSize, pageCount) {
				var pages = [];
				for (var i = 1; i <= pageCount; i++) {
					var page = i;
					if (IsPageIncluded(page, pageCount))
						pages.push(page);
				}
				return pages;
			}

			function AssignPageNumbers(pageButtons, totalRecordsCount, pageSize) {
				pageButtons.removeClass("btn-default");
				pageButtons.removeClass("btn-primary");
				var pageCount = Math.ceil(totalRecordsCount / pageSize);
				var pageNumbers = BuildPages(pageSize, pageCount);
				for (var i = 0; i < pageButtons.length; i++) {
					var pageButton = pageButtons.eq(i);
					if (i < pageNumbers.length) {
						var pageNumber = pageNumbers[i];
						var buttonClass = (pageNumber == curPage) ? "btn-primary" : "btn-default";
						pageButton.text(pageNumber);
						pageButton.addClass(buttonClass);
						pageButton.show();
					} else {
						pageButton.hide();
					}
				}
			}

			var tableContainer = pagesPlaceholder.parents(".paged-table-container").first();
			var pagedTable = tableContainer.find(".paged-table");
			var gotSearch = PagedTable.GetSearchTerm(tableContainer).length > 0;
			var allRows = (gotSearch) ? pagedTable.find(".search-result") : pagedTable.find("tr:gt(0)");
			var prevPageButton = pagesPlaceholder.find(".prev-page");
			var nextPageButton = pagesPlaceholder.find(".next-page");
			var pageButtons = pagesPlaceholder.find(".page");
			var pageSize = PagedTable.GetPageSize(tableContainer);
			var totalRecordsCount = PagedTable.GetRowCount(tableContainer).AllRowsCount;
			if (pageSize > 0 && pageSize < totalRecordsCount) {
				AssignPageNumbers(pageButtons, totalRecordsCount, pageSize);
				prevPageButton.prop('disabled', pageButtons.first().hasClass("btn-primary"));
				nextPageButton.prop('disabled', pageButtons.filter(function () { return $(this).is(":visible"); }).last().hasClass("btn-primary"));
				var firstIndex = (curPage - 1) * pageSize;
				var lastIndex = firstIndex + pageSize;
				if (lastIndex >= allRows.length)
					lastIndex = allRows.length;
				allRows.hide();
				allRows.slice(firstIndex, lastIndex).show();
				pagesPlaceholder.show();
			} else {
				allRows.show();
				pagesPlaceholder.hide();
			}
			PagedTable.HandleRecordsCount();
		},
		PageClicked: function (event) {
			var pageButton = $(event.target);
			var pagesPlaceholder = pageButton.parents(".pages-placeholder").first();
			var pageNumber = parseInt(pageButton.text());
			if (isNaN(pageNumber)) {
				var pageButtons = pagesPlaceholder.find(".page");
				pageNumber = parseInt(pageButtons.filter(function () { return $(this).hasClass("btn-primary"); }).first().text());
			}
			var jumpValue = parseInt(pageButton.data("page-jump"));
			if (!isNaN(jumpValue))
				pageNumber += jumpValue;
			PagedTable.Pager.SetCurrentPage(pagesPlaceholder, pageNumber);
		},
		PageSizeClicked: function (event) {
			var pageSizeButton = $(event.target);
			var pageSizePlaceholder = pageSizeButton.parents(".page-size-placeholder").first();
			var tableContainer = pageSizePlaceholder.parents(".paged-table-container").first();
			var selectedPageSize = parseInt(pageSizeButton.text());
			tableContainer.find("input.page-size").first().val(selectedPageSize.toString());
			PagedTable.Pager.SetCurrentPage(tableContainer.find(".pages-placeholder"), 1);
			pageSizePlaceholder.find(".page-size").removeClass("btn-success").addClass("btn-default");
			pageSizeButton.removeClass("btn-default").addClass("btn-success");
		},
		HandlePageSizes: function () {
			$(".page-size-placeholder").each(function () {
				var pageSizePlaceholder = $(this);
				var tableContainer = pageSizePlaceholder.parents(".paged-table-container").first();
				var selectedPageSize = PagedTable.GetPageSize(tableContainer);
				var pageSizeButtons = pageSizePlaceholder.find(".page-size");
				var matchingButtons = pageSizeButtons.filter(function () { return parseInt($(this).text()) == selectedPageSize; });
				if (matchingButtons.length == 1)
					matchingButtons.first().removeClass("btn-default").addClass("btn-success");
				pageSizeButtons.bind("click", PagedTable.Pager.PageSizeClicked);
			});
		},
		Init: function () {
			$(".pages-placeholder").each(function () {
				var pagesPlaceholder = $(this);
				pagesPlaceholder.find(".page,.prev-page,.next-page").bind("click", PagedTable.Pager.PageClicked);
				PagedTable.Pager.SetCurrentPage(pagesPlaceholder, 1);
			});
			this.HandlePageSizes();
		}
	},
	Init: function () {
		this.HandleRecordsCount();
		this.HandleColumnSorting();
		this.HandleRowLinks();
		this.HandleSearch();
		this.HandleAdditionalButtons();
		this.Pager.Init();
		$(".paged-table-container .add-new").bind("click", function () {
			var targetUrl = $(this).data("url") || "";
			if (targetUrl.length > 0)
				window.location.href = targetUrl;
		});
	}
};

$(document).ready(function () {
	PagedTable.Init();
});